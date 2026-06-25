FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/HrManagement.Domain/HrManagement.Domain.csproj", "src/HrManagement.Domain/"]
COPY ["src/HrManagement.Application/HrManagement.Application.csproj", "src/HrManagement.Application/"]
COPY ["src/HrManagement.Infrastructure/HrManagement.Infrastructure.csproj", "src/HrManagement.Infrastructure/"]
COPY ["src/HrManagement.Api/HrManagement.Api.csproj", "src/HrManagement.Api/"]
RUN dotnet restore "src/HrManagement.Api/HrManagement.Api.csproj"
COPY . .
RUN dotnet publish "src/HrManagement.Api/HrManagement.Api.csproj" -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "HrManagement.Api.dll"]
