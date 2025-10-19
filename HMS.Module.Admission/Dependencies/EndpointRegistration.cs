// HMS.Module.Admission/Dependencies/EndpointRegistration.cs
using HMS.Module.Admission.Features.Admission.Endpoints;
using Microsoft.AspNetCore.Routing;

namespace HMS.Module.Admission.Dependencies;

public static class EndpointRegistration
{
    public static IEndpointRouteBuilder MapAdmissionModule(this IEndpointRouteBuilder app)
    {
        app.MapAdmissionEndpoints();
        app.MapWardEndpoints();
        app.MapRoomTypeEndpoints();
        app.MapWardRoomEndpoints();
        return app;
    }
}
