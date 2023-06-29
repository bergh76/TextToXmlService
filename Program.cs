using TextToXmlService.Helpers;
using TextToXmlService.Services;

namespace TextToXmlService;

public class Program
{
    public static void Main(string[] args)
    {
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

            });
    }

   
}