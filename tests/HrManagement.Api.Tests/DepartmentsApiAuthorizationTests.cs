using System.Net;
using Xunit;

namespace HrManagement.Api.Tests;

public sealed class DepartmentsApiAuthorizationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public DepartmentsApiAuthorizationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task AnonymousRequest_IsForbidden()
    {
        var response = await _client.GetAsync("/api/departments");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
