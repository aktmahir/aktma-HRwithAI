using AspNetCoreRateLimit;
using HrManagement.Application;
using HrManagement.Infrastructure;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Serilog configuration - reads from appsettings.json
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddControllers();

// Response Compression
builder.Services.AddResponseCompression(options =>
{
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
    options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "application/json" });
});

// API Versioning
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});

// CORS - strict whitelist, specific methods and headers only
var allowedOriginsCsv = builder.Configuration["AllowedOrigins"] ?? "";
var allowedOrigins = allowedOriginsCsv.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                    .Select(o => o.Trim())
                                    .ToArray();

builder.Services.AddCors(options =>
{
    options.AddPolicy("StrictCors", builder =>
    {
        if (allowedOrigins.Length > 0)
        {
            builder.WithOrigins(allowedOrigins)
                   .WithMethods("GET", "POST", "PUT", "DELETE", "OPTIONS")
                   .WithHeaders("Content-Type", "Authorization", "X-Requested-With", "Accept", "Origin")
                   .AllowCredentials()
                   .SetPreflightMaxAge(TimeSpan.FromMinutes(10));
        }
    });
});

// Health Checks
var dbConnectionString = builder.Configuration.GetConnectionString("HrDatabase") ?? string.Empty;
builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy())
    .AddNpgSql(dbConnectionString, name: "postgresql")
    .AddUrlGroup(new Uri(builder.Configuration["Llm:BaseUrl"] + "/api/version"), name: "llm-check")
    .AddCheck("ready", () => HealthCheckResult.Healthy(), tags: new[] { "ready" });

// Rate Limiting
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
builder.Services.AddInMemoryRateLimiting();

// Swagger (only in Development)
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
}

// Application & Infrastructure
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Migration service (for startup-only mode)
builder.Services.AddSingleton<HrManagement.Infrastructure.Persistence.DatabaseInitializer>();

var app = builder.Build();

// Apply pending migrations on startup (only in non-dev environments if desired)
await app.Services.GetRequiredService<HrManagement.Infrastructure.Persistence.DatabaseInitializer>().InitializeAsync();

var runMigrationsOnly = builder.Configuration.GetValue<bool>("RunMigrationsOnly");
if (runMigrationsOnly)
{
    Log.Information("Migrations applied. Exiting.");
    return;
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Global middleware
app.UseMiddleware<HrManagement.Api.Middleware.ExceptionHandlingMiddleware>();

// Response compression
app.UseResponseCompression();

// Security Headers (HSTS, CSP, X-Frame-Options, Referrer-Policy)
app.Use(async (ctx, next) =>
{
    ctx.Response.Headers["X-Frame-Options"] = "DENY";
    ctx.Response.Headers["X-Content-Type-Options"] = "nosniff";
    ctx.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
    ctx.Response.Headers["Content-Security-Policy"] = "default-src 'self'; script-src 'self' 'unsafe-inline'; style-src 'self' 'unsafe-inline'";
    
    // HSTS - only in Production (TLS termination at reverse proxy)
    if (builder.Environment.IsProduction())
    {
        ctx.Response.Headers["Strict-Transport-Security"] = "max-age=31536000; includeSubDomains; preload";
    }
    
    await next();
});

// CORS - strict policy
app.UseCors("StrictCors");

// Authorization (if any)
app.UseAuthorization();

// Health checks (liveness - checks DB and LLM)
app.MapHealthChecks("/health");

// Readiness probe (internal only - app ready to accept traffic)
app.MapHealthChecks("/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready")
});

// Simple Prometheus metrics endpoint
app.MapGet("/metrics", () =>
{
    var process = System.Diagnostics.Process.GetCurrentProcess();
    var metrics = new[]
    {
        "# HELP dotnet_process_memory_bytes Process memory in bytes",
        "# TYPE dotnet_process_memory_bytes gauge",
        $"dotnet_process_memory_bytes {process.WorkingSet64}",
        "",
        "# HELP dotnet_process_cpu_seconds_total CPU time in seconds",
        "# TYPE dotnet_process_cpu_seconds_total counter",
        $"dotnet_process_cpu_seconds_total {process.TotalProcessorTime.TotalSeconds}",
    };
    return Results.Ok(string.Join("\n", metrics));
});

// Rate limiting
app.UseIpRateLimiting();

app.MapControllers();

app.Run();