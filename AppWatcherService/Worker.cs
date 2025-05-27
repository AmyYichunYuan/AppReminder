using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using NLog;

public class Worker : BackgroundService
{
    private readonly IConfiguration _config;
    private ManagementEventWatcher _watcher;
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public Worker(IConfiguration config)
    {
        _config = config;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            string watchedApp = _config["WatchedApp"] ?? "";
            string messageAppPath = _config["MessageAppPath"] ?? "";
            int.TryParse(_config["ActiveHours:StartHour"], out var startHour);
            int.TryParse(_config["ActiveHours:EndHour"], out var endHour);
            var activeDays = _config.GetSection("ActiveDays")?.Get<string[]>();
            string messageToShow = _config["MessageText"] ?? "Hello!";

            if (!File.Exists(messageAppPath))
            {
                Logger.Error($"Message app not found at path: {messageAppPath}");
                throw new FileNotFoundException("Message app not found.", messageAppPath);
            }

            _watcher = new ManagementEventWatcher("SELECT * FROM Win32_ProcessStartTrace");
            _watcher.EventArrived += (sender, e) =>
            {
                try
                {
                    string processName = e.NewEvent["ProcessName"]?.ToString();
                    if (string.Equals(processName, watchedApp, StringComparison.OrdinalIgnoreCase))
                    {
                        DateTime now = DateTime.Now;
                        bool isActiveDay = activeDays?.Contains(now.DayOfWeek.ToString(), StringComparer.OrdinalIgnoreCase) ?? true;
                        bool isActiveHour = (startHour == 0 || now.Hour >= startHour) && (endHour == 0 || now.Hour < endHour);

                        Logger.Info($"Detected launch of {watchedApp} at {now}. ActiveDay={isActiveDay}, ActiveHour={isActiveHour}");

                        if (isActiveDay && isActiveHour)
                        {
                            Logger.Info("Launching message app...");
                            Process.Start(new ProcessStartInfo
                            {
                                FileName = messageAppPath,
                                Arguments = $"\"{messageToShow}\"",
                                UseShellExecute = true
                            });
                        }
                        else
                        {
                            Logger.Info("Conditions not met. No message shown.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "Error handling process start event.");
                }
            };
            _watcher.Start();
            Logger.Info("Watcher started successfully.");
        }
        catch (Exception ex)
        {
            Logger.Fatal(ex, "Service encountered a fatal error during startup.");
        }

        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        _watcher?.Stop();
        _watcher?.Dispose();
        base.Dispose();
        Logger.Info("Watcher stopped and service disposed.");
        LogManager.Shutdown();
    }
}
