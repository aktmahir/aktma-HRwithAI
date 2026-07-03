using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using HrManagement.Api.Controllers;
using Xunit;

namespace HrManagement.Api.Tests;

public sealed class AiEndpointsApiTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AiEndpointsApiTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task ScreenCv_WithBlankPayload_ReturnsBadRequest()
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "/api/ai/cv-screening");
        request.Headers.Authorization = new AuthenticationHeaderValue("TestScheme");
        request.Content = JsonContent.Create(new ScreenCvRequest("   ", "test"));

        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task AnalyzeReview_WithBlankPayload_ReturnsBadRequest()
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "/api/ai/performance-review-analysis");
        request.Headers.Authorization = new AuthenticationHeaderValue("TestScheme");
        request.Content = JsonContent.Create(new AnalyzePerformanceReviewRequest("   "));

        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
