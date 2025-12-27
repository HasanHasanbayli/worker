using Hangfire;
using Worker.Application;
using Worker.Infrastructure;
using Worker.Infrastructure.Hangfire;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddWorkerApplication();
builder.Services.AddOpenApi();

builder.Services.AddHangfire(config =>
    config.UseSqlServerStorage(builder.Configuration.GetConnectionString("HangfireConnection")));

builder.Services.AddHangfireServer();

builder.Services.AddHealthChecks();

// App layers
builder.Services
    .AddWorkerApplication()
    .AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseHangfireDashboard("", new DashboardOptions());

// Register recurring jobs
using (var scope = app.Services.CreateScope())
{
    var manager = scope.ServiceProvider.GetRequiredService<HangfireRecurringJobScheduler>();

    manager.RegisterAll();
}

app.UseHealthChecks("/health");

app.Run();