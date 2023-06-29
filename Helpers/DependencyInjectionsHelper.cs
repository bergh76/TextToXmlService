using TextToXmlService.Classes;

namespace TextToXmlService.Helpers;

public static class DependencyInjectionsHelper
{
    /// <summary>
    /// Configure IoC
    /// </summary>
    /// <param name="services"></param>
    public static void Configure(IServiceCollection services)
    {
        services.AddScoped<IStringSerializer, XmlSerializer>();
        services.AddScoped<IXmlFileCreator, XmlFileCreator>();
    }
}