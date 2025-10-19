using Microsoft.AspNetCore.Mvc.RazorPages;
using HMS.Module.Lab.Features.Lab.Dashboard;
using HMS.Module.Lab.Infrastructure.Persistence;

namespace HMS.Api.Pages.Lab.LabRequests;

public sealed class OrdersIndexModel : PageModel
{
    private readonly ILabDashboardService _svc;
    public IReadOnlyList<DashOrderRowDto> Items { get; private set; } = Array.Empty<DashOrderRowDto>();
    public OrdersIndexModel(ILabDashboardService svc) => _svc = svc;
    public async Task OnGet(CancellationToken ct) => Items = await _svc.GetOrdersAsync(ct);
}
