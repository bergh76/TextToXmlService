using TextToXmlService.Models;

namespace TextToXmlService.Services;

public static class OptionsConfigurationsHelper
{
    /// <summary>
    /// Configure IOption from appsetting.json
    /// </summary>
    /// <param name="hostContext"></param>
    /// <param name="services"></param>
    public static void Configure(HostBuilderContext hostContext, IServiceCollection services)
    {
        var configurationRoot = hostContext.Configuration;

        // Set up FilePathOptions
        services.Configure<FilePathOptions>(configurationRoot.GetSection(nameof(FilePathOptions)));
    }
}