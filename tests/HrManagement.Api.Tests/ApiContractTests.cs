using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Xunit;

namespace HrManagement.Api.Tests;

public class ApiContractTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ApiContractTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Theory]
    [InlineData("/api/auth/login", "POST")]
    [InlineData("/api/employees", "GET")]
    [InlineData("/api/employees", "POST")]
    [InlineData("/api/departments", "GET")]
    [InlineData("/api/departments", "POST")]
    [InlineData("/api/roles", "GET")]
    [InlineData("/api/roles", "POST")]
    [InlineData("/api/leave-requests", "GET")]
    [InlineData("/api/leave-requests", "POST")]
    [InlineData("/api/ai/cv-screening", "POST")]
    [InlineData("/api/ai/performance-review-analysis", "POST")]
    public async Task KnownEndpoints_ShouldExist(string relativeUrl, string method)
    {
        var request = new HttpRequestMessage(new HttpMethod(method), relativeUrl);
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("TestScheme");

        var response = await _client.SendAsync(request);

        Assert.NotEqual(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }
}
