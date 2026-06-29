# Use alpine for smaller base image (~100MB vs ~250MB)
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS base
# Install curl for healthcheck, ca-certificates for HTTPS, and tzdata for time zone support
RUN apk add --no-cache curl ca-certificates tzdata
WORKDIR /app
# Create non-root user
RUN addgroup -g 10001 -S appgroup && adduser -u 10001 -S -G appgroup appuser
USER appuser
EXPOSE 8080
HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 CMD curl -f http://localhost:8080/health || exit 1

# Build stage uses SDK image
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /src
COPY ["src/HrManagement.Domain/HrManagement.Domain.csproj", "src/HrManagement.Domain/"]
COPY ["src/HrManagement.Application/HrManagement.Application.csproj", "src/HrManagement.Application/"]
COPY ["src/HrManagement.Infrastructure/HrManagement.Infrastructure.csproj", "src/HrManagement.Infrastructure/"]
COPY ["src/HrManagement.Api/HrManagement.Api.csproj", "src/HrManagement.Api/"]
RUN dotnet restore "src/HrManagement.Api/HrManagement.Api.csproj"
COPY . .
# Trimming disabled for EF Core compatibility
RUN dotnet publish "src/HrManagement.Api/HrManagement.Api.csproj" -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
RUN chown -R appuser /app
ENTRYPOINT ["dotnet", "HrManagement.Api.dll"]