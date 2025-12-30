using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Worker.Application.Ports;
using Worker.Infrastructure.Hangfire;
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

        services.AddInfrastructureLogging(configuration);
        services.AddInfrastructurePersistence(configuration);
        services.AddHangfireInfrastructure();

        return services;
    }

    private static IServiceCollection AddInfrastructureLogging(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .WriteTo.Console()
            .WriteTo.File(
                path: "log-.txt",
                rollingInterval: RollingInterval.Day)
            .CreateLogger();

        services.AddSingleton(typeof(ILoggerService<>), typeof(LoggerService<>));

        return services;
    }

    private static IServiceCollection AddInfrastructurePersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("HangfireConnection")!;

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

        // Recurring jobs

        return services;
    }
}