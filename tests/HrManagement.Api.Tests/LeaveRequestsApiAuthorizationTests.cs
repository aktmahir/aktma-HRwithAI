using System.Net;
using Xunit;

namespace HrManagement.Api.Tests;

public sealed class LeaveRequestsApiAuthorizationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public LeaveRequestsApiAuthorizationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task AnonymousRequest_IsUnauthorized()
    {
        var response = await _client.GetAsync("/api/leave-requests");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
