using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Worker.Application.Ports;
using Worker.Infrastructure.Hangfire;
using Worker.Infrastructure.Hangfire.Jobs;
using Worker.Infrastructure.Persistence;
using Worker.Infrastructure.Services;

namespace Worker.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHostedService<JobStateInitializerHostedService>();
        services.AddScoped<HangfireRecurringJobScheduler>();

        // Register Infrastructure Services
        services.AddScoped<IDbCommandExecutor, SqlCommandExecutor>();
        services.AddScoped<IJobStateService, SqlJobStateService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<ISmsService, SmsService>();

        // Register Recurring Jobs
        services.AddScoped<IRecurringJob, EmailJob>();
        services.AddScoped<IRecurringJob, SmsJob>();
        services.AddScoped<IRecurringJob, CustomLogicJob>();

        return services;
    }
}