using HMS.Module.Lab.Infrastructure.Persistence; // if you keep namespaces grouped nearby
using Microsoft.Extensions.DependencyInjection;

namespace HMS.Api.Extensions;

public static class LabRazorRoutesExtensions
{
    /// <summary>
    /// Registers Razor Pages and applies all Lab-related page routes.
    /// </summary>
    public static IServiceCollection AddLabRazorRoutes(this IServiceCollection services)
    {
        services.AddRazorPages(o =>
        {
            // dashboard
            o.Conventions.AddPageRoute("/Lab/Dashboard/Index", "lab");
            o.Conventions.AddPageRoute("/Lab/Dashboard/Index", "lab/dashboard");

            // lists
            o.Conventions.AddPageRoute("/Lab/Dashboard/Orders", "lab/orders");
            o.Conventions.AddPageRoute("/Lab/Dashboard/Results", "lab/results");
            o.Conventions.AddPageRoute("/Lab/Dashboard/Samples", "lab/samples");

            // catalog
            o.Conventions.AddPageRoute("/Lab/Category/Tests", "lab/catalog/tests");
            o.Conventions.AddPageRoute("/Lab/Category/Panels", "lab/catalog/panels");
            o.Conventions.AddPageRoute("/Lab/Category/InstrumentMaps", "lab/catalog/instrument-maps");

            // tools
            o.Conventions.AddPageRoute("/Barcodes/Generate", "barcodes/generate");
            o.Conventions.AddPageRoute("/Lab/Tools/CommBench", "comm/bench"); // optional: if you have a page for it
        });

        return services;
    }
}
