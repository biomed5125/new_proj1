using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Threading;
using System.Threading.Tasks;

namespace HMS.Module.Lab.Features.Lab.Dashboard;

public static class LabDashboardEndpoints
{
    public static IEndpointRouteBuilder MapLabDashboard(this IEndpointRouteBuilder routes)
    {
        var g = routes.MapGroup("/api/lab/dashboard");

        g.MapGet("/orders", async (ILabDashboardService svc, CancellationToken ct)
            => Results.Ok(await svc.GetOrdersAsync(ct)));

        g.MapGet("/order/{labRequestId:long}", async (long labRequestId, ILabDashboardService svc, CancellationToken ct)
            => Results.Ok(await svc.GetOrderAsync(labRequestId, ct)));

        g.MapGet("/samples/pending", async (ILabDashboardService svc, CancellationToken ct)
            => Results.Ok(await svc.GetPendingSamplesAsync(ct)));

        g.MapGet("/results", async (ILabDashboardService svc, CancellationToken ct)
            => Results.Ok(await svc.GetResultsAsync(ct)));

        g.MapGet("/result/history/{labRequestItemId:long}", async (long labRequestItemId, ILabDashboardService svc, CancellationToken ct)
            => Results.Ok(await svc.GetResultHistoryAsync(labRequestItemId, ct)));

        g.MapGet("/summary", async (ILabDashboardService svc, CancellationToken ct)
            => Results.Ok(await svc.GetSummaryAsync(ct)));

        return routes;
    }
}
