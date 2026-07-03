using Xunit;

namespace HrManagement.Api.Tests;

public sealed class JwtConfigurationTests
{
    [Fact]
    public void JwtOptions_RequiresSecretIssuerAndAudience()
    {
        var options = new JwtOptions();

        Assert.True(string.IsNullOrWhiteSpace(options.Secret));
        Assert.True(string.IsNullOrWhiteSpace(options.Issuer));
        Assert.True(string.IsNullOrWhiteSpace(options.Audience));
    }
}
