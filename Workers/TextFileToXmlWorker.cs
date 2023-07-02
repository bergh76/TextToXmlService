using Quartz;
using Quartz.Util;
using TextToXmlService.Classes;

namespace TextToXmlService.Workers;

[DisallowConcurrentExecution]
public class TextFileToXmlJob : IJob
{
    private readonly ILogger<TextFileToXmlJob> _logger;
    private readonly IXmlFileCreator _xmlFileCreator;
    private readonly ITextFileHelper _textFileHelper;

    public TextFileToXmlJob(ILogger<TextFileToXmlJob> logger, IXmlFileCreator fileCreator, ITextFileHelper textFileHelper)

    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _xmlFileCreator = fileCreator ?? throw new ArgumentNullException(nameof(fileCreator));
        _textFileHelper = textFileHelper ?? throw new ArgumentNullException(nameof(textFileHelper));
    }

    public Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("Begin Work!");

        var foldersExists = _textFileHelper.CreateFolders();
        foreach (var keyValuePair in foldersExists)
        {
            if (keyValuePair.Value == false)
            {
                _logger.LogError("Missing folders for {Message}", keyValuePair.Key);
                return Task.CompletedTask;
            }

            _logger.LogInformation("{Message}", keyValuePair.Key);
        }
        
        var fileNames = _textFileHelper.GetFiles();
        _logger.LogInformation("Files to handle {TotalFiles}.", fileNames.Count);
        if (!fileNames.Any())
        {
            _logger.LogInformation("DONE. No files to handle");
            return Task.CompletedTask;
        }

        foreach (var fileName in fileNames)
        {
            var people = _textFileHelper.ReadTextFile(fileName);
            if (people != null)
            {
                // Write the xml file.
                _xmlFileCreator.WriteXmlFile(fileName, people);

                // When we are done we move the file to archive folder.
                _textFileHelper.MoveFile(fileName);
            }
        }
        _logger.LogInformation("DONE. Total of {NoOfFiles} xml files created!", fileNames.Count);
        return Task.CompletedTask;
    }
}