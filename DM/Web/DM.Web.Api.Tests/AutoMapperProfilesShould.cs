using AutoMapper;
using DM.Web.API;
using Xunit;

namespace DM.Web.Api.Tests;

/// <summary>
/// Auto mapper profiles test
/// </summary>
public sealed class AutoMapperProfilesShould
{
    /// <summary>
    /// Check auto mapper profiles
    /// </summary>
    [Fact]
    public void BeConfiguredCorrectly()
    {
        var configuration = new MapperConfiguration(cfg => cfg.AddMaps(typeof(Startup).Assembly));
        configuration.AssertConfigurationIsValid();
    }
}
