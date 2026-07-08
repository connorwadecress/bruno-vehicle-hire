using BrunoVehicleHire.Application;

namespace BrunoVehicleHire.Application.Tests;

public class ArchitectureSmokeTests
{
    [Fact]
    public void ApplicationProject_IsLoadable()
    {
        var assembly = typeof(ApplicationAssemblyMarker).Assembly;

        Assert.Equal("BrunoVehicleHire.Application", assembly.GetName().Name);
    }
}
