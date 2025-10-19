// HMS.Communication.Infrastructure/Drivers/AnalyzerDriverResolver.cs
using HMS.Communication.Abstractions;
using HMS.Communication.Infrastructure.Persistence.Entities;

namespace HMS.Communication.Infrastructure.Drivers;

public interface IAnalyzerDriverResolver
{
    (IAnalyzerDriver Driver, string DeviceCode, string? Mode) Resolve(CommDevice device);
}

public sealed class AnalyzerDriverResolver : IAnalyzerDriverResolver
{
    private readonly IAnalyzerDriverFactory _factory;
    public AnalyzerDriverResolver(IAnalyzerDriverFactory factory) => _factory = factory;

    public (IAnalyzerDriver Driver, string DeviceCode, string? Mode) Resolve(CommDevice device)
    {
        var d = _factory.Create(device);
        var code = string.IsNullOrWhiteSpace(device.DeviceCode) ? "DEV" : device.DeviceCode.Trim();
        var mode = device.AnalyzerProfile?.DefaultMode;
        return (d, code, mode);
    }
}
