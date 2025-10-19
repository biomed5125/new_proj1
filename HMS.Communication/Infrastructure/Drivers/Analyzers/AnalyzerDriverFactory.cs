// HMS.Communication.Infrastructure/Drivers/AnalyzerDriverFactory.cs
using HMS.Communication.Abstractions;
using HMS.Communication.Infrastructure.Persistence.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace HMS.Communication.Infrastructure.Drivers;

public interface IAnalyzerDriverFactory
{
    IAnalyzerDriver Create(CommDevice device);
}

public sealed class AnalyzerDriverFactory : IAnalyzerDriverFactory
{
    private readonly IServiceProvider _sp;
    public AnalyzerDriverFactory(IServiceProvider sp) => _sp = sp;

    public IAnalyzerDriver Create(CommDevice device)
    {
        var klass = device.AnalyzerProfile?.DriverClass?.Trim() ?? "";
        var proto = device.AnalyzerProfile?.Protocol?.Trim().ToUpperInvariant() ?? "ASTM";

        // Prefer explicit class name
        if (klass.Contains("Roche", StringComparison.OrdinalIgnoreCase))
            return ActivatorUtilities.CreateInstance<Analyzers.RocheCobasDriver>(_sp);

        if (klass.Contains("Sysmex", StringComparison.OrdinalIgnoreCase))
            return ActivatorUtilities.CreateInstance<Analyzers.SysmexSuitDriver>(_sp);

        if (klass.Contains("Fuji", StringComparison.OrdinalIgnoreCase))
            return ActivatorUtilities.CreateInstance<Analyzers.FujiDryChemDriver>(_sp);

        // Fallback by protocol
        return proto switch
        {
            "SUIT" => ActivatorUtilities.CreateInstance<Analyzers.SysmexSuitDriver>(_sp),
            "ASCII" => ActivatorUtilities.CreateInstance<Analyzers.FujiDryChemDriver>(_sp),
            _ => ActivatorUtilities.CreateInstance<Analyzers.RocheCobasDriver>(_sp),
        };
    }
}

