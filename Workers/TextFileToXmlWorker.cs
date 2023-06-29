using Microsoft.Extensions.Options;
using Quartz;
using TextToXmlService.Classes;
using TextToXmlService.Models;

namespace TextToXmlService.Workers;

[DisallowConcurrentExecution]
public class TextFileToXmlJob : IJob
{
    private readonly ILogger<TextFileToXmlJob> _logger;
    private readonly IXmlFileCreator _fileCreator;

    public TextFileToXmlJob(ILogger<TextFileToXmlJob> logger, IXmlFileCreator fileCreator)

    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _fileCreator = fileCreator ?? throw new ArgumentNullException(nameof(fileCreator));
    }

    public Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("Begin Work!");

        _fileCreator.WriteXmlFile();

        return Task.CompletedTask;
    }
}