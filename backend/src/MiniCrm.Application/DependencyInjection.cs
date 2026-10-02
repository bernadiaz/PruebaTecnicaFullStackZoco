using Microsoft.Extensions.DependencyInjection;
using MiniCrm.Application.Services;

namespace MiniCrm.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ClienteService>();
        services.AddScoped<GestionService>();
        services.AddScoped<DashboardService>();
        services.AddScoped<AsesorService>();
        return services;
    }
}
