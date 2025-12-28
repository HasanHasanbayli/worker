using Hangfire;
using Worker.Application;
using Worker.Infrastructure;
using Worker.Infrastructure.Hangfire;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
IServiceCollection services = builder.Services;
IConfiguration configuration = builder.Configuration;

services.AddHangfire(config =>
    config.UseSqlServerStorage(configuration.GetConnectionString("HangfireConnection")));

services.AddHangfireServer();
services.AddHealthChecks();

// App layers
services.AddWorkerApplication();
services.AddInfrastructure(configuration);

WebApplication app = builder.Build();

app.UseHangfireDashboard("", new DashboardOptions());

// Register recurring jobs
using (var scope = app.Services.CreateScope())
{
    var manager = scope.ServiceProvider.GetRequiredService<HangfireRecurringJobScheduler>();

    manager.RegisterAll();
}

app.UseHealthChecks("/health");

app.Run();