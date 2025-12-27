using Microsoft.Extensions.DependencyInjection;
using Worker.Application.Jobs;
using Worker.Application.Scheduling;

namespace Worker.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddWorkerApplication(this IServiceCollection services)
    {
        // Register Recurring Jobs
        services.AddScoped<IRecurringJob, EmailJob>();
        services.AddScoped<IRecurringJob, SmsJob>();

        return services;
    }
}