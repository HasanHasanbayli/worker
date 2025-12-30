using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Worker.Application.Ports;
using Worker.Infrastructure.Hangfire;
using Worker.Infrastructure.Hangfire.Jobs;
using Worker.Infrastructure.Persistence;
using Worker.Infrastructure.Services;

namespace Worker.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddHttpClient();

        services.AddLoggingInfrastructure(configuration);
        services.AddPersistenceInfrastructure(configuration);
        services.AddHangfireInfrastructure();
        services.AddJobs();

        // Services
        services.AddScoped<ISmsService, SmsService>();
        services.AddScoped<IEmailService, EmailService>();

        return services;
    }

    private static IServiceCollection AddLoggingInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        Log.Logger = CreateLogger(configuration);

        services.AddSingleton(typeof(ILoggerService<>), typeof(LoggerService<>));

        return services;
    }

    private static ILogger CreateLogger(IConfiguration configuration)
    {
        return new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .WriteTo.Console()
            .WriteTo.File(
                path: "log-.txt",
                rollingInterval: RollingInterval.Day)
            .CreateLogger();
    }

    private static IServiceCollection AddPersistenceInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString =
            configuration.GetConnectionString("HangfireConnection")
            ?? throw new InvalidOperationException(
                "Connection string 'HangfireConnection' is missing.");

        services.AddSingleton<ISqlSessionFactory>(_ => new SqlSessionFactory(connectionString));

        return services;
    }

    private static IServiceCollection AddHangfireInfrastructure(
        this IServiceCollection services)
    {
        // Hosted services
        services.AddHostedService<JobStateInitializerHostedService>();

        // Infrastructure services
        services.AddScoped<HangfireRecurringJobScheduler>();
        services.AddScoped<IJobStateService, SqlJobStateService>();

        return services;
    }

    private static IServiceCollection AddJobs(
        this IServiceCollection services)
    {
        services.AddScoped<IRecurringJob, EmailJob>();
        services.AddScoped<IRecurringJob, SmsJob>();
        services.AddScoped<IRecurringJob, CustomLogicJob>();

        return services;
    }
}