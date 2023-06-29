using TextToXmlService.Classes;
using TextToXmlService.Workers;

namespace TextToXmlService.Helpers;

public static class DependencyInjectionsHelper
{

    /// <summary>
    /// Configure IoC
    /// </summary>
    /// <param name="services"></param>
    public static void Configure(IServiceCollection services)
    {

        services.AddHostedService<Worker>()
                .AddScoped<IStringSerializer, XmlSerializer>();
    }
}
