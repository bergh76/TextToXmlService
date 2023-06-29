using TextToXmlService.Helpers;
using TextToXmlService.Services;
using Serilog;
using Serilog.Events;

namespace TextToXmlService;

public class Program
{
    public static void Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateLogger();
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