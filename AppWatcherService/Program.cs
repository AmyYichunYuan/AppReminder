using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.WindowsServices;
using NLog;
using NLog.Extensions.Logging;

var logger = LogManager.Setup().LoadConfigurationFromFile("NLog.config").GetCurrentClassLogger();

try
{
    logger.Info("Starting AppWatcherService...");

    bool isService = !(System.Diagnostics.Debugger.IsAttached || args.Contains("--console"));

    var builder = Host.CreateDefaultBuilder(args)
        .ConfigureAppConfiguration((hostingContext, config) =>
        {
            config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                  .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true);
        })
        .ConfigureLogging(logging =>
        {
            logging.ClearProviders();
            logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
            logging.AddNLog("NLog.config"); // Ensure nlog.config is copied to output
        })
        .ConfigureServices((hostContext, services) =>
        {
            services.AddHostedService<Worker>();
        });

    if (isService)
    {
        builder.UseWindowsService(); // This makes it run as a Windows Service
    }
    else
    {
        Console.WriteLine("Running as Console Application...");
    }
    
    builder.Build().Run();
}
catch (Exception ex)
{
    logger.Fatal(ex, "Service terminated unexpectedly.");
}
finally
{
    LogManager.Shutdown();
}