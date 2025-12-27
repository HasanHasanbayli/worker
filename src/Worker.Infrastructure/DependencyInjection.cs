using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Worker.Application.JobState;
using Worker.Application.Persistence;
using Worker.Application.Services;
using Worker.Infrastructure.Hangfire;
using Worker.Infrastructure.Persistence;
using Worker.Infrastructure.Services;
using SqlCommandExecutor = Worker.Infrastructure.Persistence.SqlCommandExecutor;

namespace Worker.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {

        services.AddHostedService<JobStateInitializerHostedService>();
        services.AddScoped<HangfireRecurringJobScheduler>();

        services.AddScoped<IDbCommandExecutor, SqlCommandExecutor>();
        services.AddScoped<IJobStateService, SqlJobStateService>();

        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<ISmsService, SmsService>();

        return services;
    }
}