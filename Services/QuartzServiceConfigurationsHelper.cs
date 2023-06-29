using Quartz;
using TextToXmlService.Services;
using TextToXmlService.Workers;

public static class QuartzServiceConfigurationsHelper
{
    public static void Configure(HostBuilderContext hostContext, IServiceCollection services)
    {
        // Add the required Quartz.NET services
        services.AddQuartz(q =>
        {
            // Use a Scoped container to create jobs.
            q.UseMicrosoftDependencyInjectionJobFactory();


            // Register the job, loading the schedule from configuration
            q.AddJobAndTrigger<TextFileToXmlJob>(hostContext.Configuration);


        });

        // Add the Quartz.NET hosted service

        services.AddQuartzHostedService(
            q => q.WaitForJobsToComplete = true);

        // other config
    }
}