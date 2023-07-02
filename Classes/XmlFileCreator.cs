using Microsoft.Extensions.Options;
using TextToXmlService.Models;

namespace TextToXmlService.Classes;

public class XmlFileCreator : IXmlFileCreator
{
    private readonly ILogger<XmlFileCreator> _logger;
    private readonly FilePathOptions _filePathOptions;
    private readonly IStringSerializer _stringSerializer;

    public XmlFileCreator(ILogger<XmlFileCreator> logger, IOptions<FilePathOptions> filePathOptions,
        IStringSerializer stringSerializer)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _filePathOptions = filePathOptions.Value ?? throw new ArgumentNullException(nameof(filePathOptions));
        _stringSerializer = stringSerializer ?? throw new ArgumentNullException(nameof(stringSerializer));
    }

    public void WriteXmlFile(string fileName, People people)
    {
        try
        {
            _logger.LogInformation("Begin writing file.");
            var inputPath = _filePathOptions.Input;
            if (inputPath == null)
            {
                _logger.LogInformation("Input path from appsettings.json is empty");
                return;
            }

            var filePath = Path.Combine(inputPath, fileName);

            // Get the filename without extension.
            var xmlFileName = $"{Path.GetFileNameWithoutExtension(filePath)}_{Guid.NewGuid()}.xml";
            if (_filePathOptions.Output == null)
            {
                _logger.LogInformation("Output path from appsettings.json is empty");

                return;
            }

            // The path to the xml file to write.
            var xmlFilePath = Path.Combine(_filePathOptions.Output, xmlFileName);
            _logger.LogInformation("Xml file path: {XmlFilePath}", xmlFilePath);
            //Serialize the object to XML
            var serializer = _stringSerializer.Serialize<People>(people);
            using (var stream = new StreamWriter(xmlFilePath))
            {

                _logger.LogInformation("Writing XML file {FileName}", xmlFileName);
                stream.Write(serializer);
            }

            _logger.LogInformation("XML file created successfully. {FileName}", xmlFilePath);
        }
        catch (Exception ex)
        {
            _logger.LogError("Can not write XML file. {NewLine} Exception: {Message}", Environment.NewLine, ex);
        }
    }
}