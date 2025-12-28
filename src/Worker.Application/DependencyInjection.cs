using Microsoft.Extensions.DependencyInjection;
using Worker.Application.UseCases;

namespace Worker.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddWorkerApplication(this IServiceCollection services)
    {
        services.AddScoped<CalculateRetryPolicyService>();

        return services;
    }
}