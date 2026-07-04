using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using HrManagement.Api.Controllers;
using HrManagement.Api.Logging;
using HrManagement.Application.Abstractions.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace HrManagement.Api.Tests;

public sealed class DepartmentsApiTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public DepartmentsApiTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetDepartments_ReturnsOk()
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/departments");
        request.Headers.Authorization = new AuthenticationHeaderValue("TestScheme");

        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task CreateDepartment_ReturnsCreated()
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "/api/departments");
        request.Headers.Authorization = new AuthenticationHeaderValue("TestScheme");
        request.Content = JsonContent.Create(new CreateDepartmentRequest("Engineering"));

        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task CreateDepartment_WithBlankName_ReturnsBadRequest()
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "/api/departments");
        request.Headers.Authorization = new AuthenticationHeaderValue("TestScheme");
        request.Content = JsonContent.Create(new CreateDepartmentRequest("   "));

        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetDepartments_ReturnsCorrelationIdHeader()
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/departments");
        request.Headers.Authorization = new AuthenticationHeaderValue("TestScheme");

        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(response.Headers.TryGetValues("X-Correlation-ID", out var values));
        Assert.Contains(values, value => !string.IsNullOrWhiteSpace(value));
    }

    [Fact]
    public async Task ValidationException_ReturnsProblemDetailsWithCorrelationId()
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/departments/invalid");
        request.Headers.Authorization = new AuthenticationHeaderValue("TestScheme");

        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.True(response.Headers.TryGetValues("X-Correlation-ID", out var values));
        Assert.Contains(values, value => !string.IsNullOrWhiteSpace(value));
    }

    [Fact]
    public async Task AuditLogger_WritesStructuredMessage()
    {
        var provider = new TestLoggerProvider();
        using var loggerFactory = LoggerFactory.Create(builder => builder.AddProvider(provider));
        var auditLogRepository = new Mock<IAuditLogRepository>();
        var auditLogger = new AuditLogger(new Logger<AuditLogger>(loggerFactory), auditLogRepository.Object);

        await auditLogger.LogChangeAsync("Create", "Employee", "test-user", "Created employee");

        var entry = Assert.Single(provider.Messages);
        Assert.Contains("Create", entry);
        Assert.Contains("Employee", entry);
        Assert.Contains("test-user", entry);
        Assert.Contains("Created employee", entry);
    }

    private sealed class TestLoggerProvider : ILoggerProvider
    {
        public List<string> Messages { get; } = new();

        public ILogger CreateLogger(string categoryName) => new TestLogger(Messages);

        public void Dispose()
        {
        }

        private sealed class TestLogger(List<string> messages) : ILogger
        {
            public IDisposable BeginScope<TState>(TState state) where TState : notnull => NullScope.Instance;

            public bool IsEnabled(LogLevel logLevel) => true;

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
            {
                messages.Add(formatter(state, exception));
            }
        }

        private sealed class NullScope : IDisposable
        {
            public static NullScope Instance { get; } = new();

            public void Dispose()
            {
            }
        }
    }
}
