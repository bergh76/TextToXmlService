using Serilog;
using Serilog.Events;
using System.Diagnostics;
using TextToXmlService.Helpers;
using TextToXmlService.Services;

namespace TextToXmlService;

public class Program
{
    public static void Main(string[] args)
    {
        var logFilePath = Debugger.IsAttached
            ? "C:\\Temp\\TextToXml\\logs\\textToXml_log_DEV-.log"
            : "C:\\Temp\\TextToXml\\logs\\textToXml_log.log";

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateLogger();

        Log.Logger.Information(Debugger.IsAttached
                ? "You are in Debug mode {InDebug}"
                : "You are in Debug mode {NotInDebug}",
            Debugger.IsAttached);

        Log.Logger.Information("Logfile path is {LogFilePath}", logFilePath);

        CreateHostBuilder(args).Build().Run();
    }

    private static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                OptionsConfigurationsHelper.Configure(hostContext, services);

                DependencyInjectionsHelper.Configure(services);
                //services.AddHostedService<Worker>();

                QuartzServiceConfigurationsHelper.Configure(hostContext, services);
            })
            .UseSerilog();
    }
}