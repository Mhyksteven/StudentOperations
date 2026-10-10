using Hangfire;
using Hangfire.SqlServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StudentOperations.Application;
using StudentOperations.ConsoleApp;
using StudentOperations.Infrastructure.Data;
using StudentOperations.Infrastructure.Services;

var builder = Host.CreateApplicationBuilder(args);

// Explicitly load appsettings.json from the application's output directory.
builder.Configuration.AddJsonFile(
    Path.Combine(AppContext.BaseDirectory, "appsettings.json"),
    optional: false,
    reloadOnChange: true);

var studentConnection =
    builder.Configuration.GetConnectionString("StudentDatabase")
    ?? throw new InvalidOperationException(
        "StudentDatabase connection string is missing.");

var hangfireConnection =
    builder.Configuration.GetConnectionString("HangfireDatabase")
    ?? throw new InvalidOperationException(
        "HangfireDatabase connection string is missing.");

builder.Services.AddDbContext<StudentOperationsDbContext>(
    options => options.UseSqlServer(studentConnection));

builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();
builder.Services.AddTransient<EnrollmentJob>();

builder.Services.AddHangfire(configuration =>
    configuration
        .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UseSqlServerStorage(
            hangfireConnection,
            new SqlServerStorageOptions()));

builder.Services.AddHangfireServer();
builder.Services.AddScoped<ConsoleMenu>();

using var host = builder.Build();

await host.StartAsync();

try
{
    RecurringJob.AddOrUpdate<EnrollmentJob>(
        "process-pending-enrollments",
        job => job.ProcessPendingAsync(),
        Cron.Minutely);

    using var scope = host.Services.CreateScope();
    var menu = scope.ServiceProvider.GetRequiredService<ConsoleMenu>();
    await menu.RunAsync();
}
finally
{
    await host.StopAsync();
}