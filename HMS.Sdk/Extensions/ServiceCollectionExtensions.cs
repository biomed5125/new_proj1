using HMS.Sdk.Clients;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HMS.Sdk.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHmsSdk(this IServiceCollection services, IConfiguration config)
    {
        var baseUrl = config["Api:BaseUrl"] ?? "http://localhost:5000/";
        services.AddHttpClient<PatientsClient>(c => c.BaseAddress = new Uri(baseUrl));
        return services;
    }
}
