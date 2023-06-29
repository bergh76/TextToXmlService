using Microsoft.Extensions.Options;
using Quartz;
using TextToXmlService.Classes;
using TextToXmlService.Models;

namespace TextToXmlService.Workers;

[DisallowConcurrentExecution]
public class TextFileToXmlJob : IJob
{
    private readonly ILogger<TextFileToXmlJob> _logger;
    private readonly FilePathOptions _filePathOptions;
    private readonly IStringSerializer _stringSerializer;

    public TextFileToXmlJob(ILogger<TextFileToXmlJob> logger, IOptions<FilePathOptions> filePathOptions,
        IStringSerializer stringSerializer)

    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _stringSerializer = stringSerializer ?? throw new ArgumentNullException(nameof(stringSerializer));
        _filePathOptions = filePathOptions.Value ?? throw new ArgumentNullException(nameof(filePathOptions));
    }

    public Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("Begin Work!");

        CreateXmlFile();

        return Task.CompletedTask;
    }


    private void CreateXmlFile()
    {
        _logger.LogInformation("Start creating XML");
        var inputPath = _filePathOptions.Input;

        var fileNames = Directory.GetFiles(inputPath);

        if (!fileNames.Any())
            _logger.LogInformation("No file to handle....");

        foreach (var fileName in fileNames)
        {
            _logger.LogInformation("Handle File: {fileName}", fileName);
            var filePath = Path.Combine(inputPath, fileName);
            var fileInfo = new FileInfo(filePath);
            var xmlFileName = $"{fileInfo.Name}.xml";
            var xmlFilePath = Path.Combine(_filePathOptions.Output, xmlFileName);
            var lines = File.ReadAllLines(filePath);

            // Create the People object
            var people = new People();
            people.Person = new List<Person>();

            // Initialize variables to hold person and family information
            Person? currentPerson = null;
            Family? currentFamily = null;

            // Iterate over lines and create People, Person, and Family objects
            foreach (var line in lines)
            {
                var parts = line.Split('|');
                var rowType = parts[0];

                switch (rowType)
                {
                    case "P":
                        currentPerson = new Person();
                        currentPerson.Firstname = parts.Length > 1
                            ? parts[1].Trim()
                            : null;
                        currentPerson.Lastname = parts.Length > 2
                            ? parts[2].Trim()
                            : null;
                        currentPerson.Address = new Address();
                        currentPerson.Phone = new Phone();
                        currentPerson.Family = new List<Family>();
                        currentFamily = null;
                        people.Person.Add(currentPerson);

                        break;
                    case "T":
                        if (currentFamily == null)
                        {
                            currentPerson.Phone.Mobile = parts.Length > 1
                                ? parts[1].Trim()
                                : null;
                            currentPerson.Phone.Landline = parts.Length > 2
                                ? parts[2].Trim()
                                : null;
                        }
                        else
                        {
                            currentFamily.Phone.Mobile = parts.Length > 1
                                ? parts[1].Trim()
                                : null;
                            currentFamily.Phone.Landline = parts.Length > 2
                                ? parts[2].Trim()
                                : null;
                        }
                        break;
                    case "A":
                        if (currentFamily == null)
                        {
                            currentPerson.Address.Street = parts.Length > 1
                                ? parts[1].Trim()
                                : null;
                            currentPerson.Address.City = parts.Length > 2
                                ? parts[2].Trim()
                                : null;
                            currentPerson.Address.Zipcode = parts.Length > 3
                                ? parts[3].Trim()
                                : null;
                        }
                        else
                        {
                            currentFamily.Address.Street = parts.Length > 1
                                ? parts[1].Trim()
                                : null;
                            currentFamily.Address.City = parts.Length > 2
                                ? parts[2].Trim()
                                : null;
                            currentFamily.Address.Zipcode = parts.Length > 3
                                ? parts[3].Trim()
                                : null;
                        }

                        break;
                    case "F":
                        currentFamily = new Family();
                        currentFamily.Name = parts.Length > 1
                            ? parts[1].Trim()
                            : null;
                        currentFamily.Born = parts.Length > 2
                            ? parts[2].Trim()
                            : null;
                        currentFamily.Address = new Address();
                        currentFamily.Phone = new Phone();
                        currentPerson?.Family.Add(currentFamily);
                        break;
                }
            }


            // Serialize the People object to XML
            var serializer = new XmlSerializer().Serialize<People>(people);
            using (var stream = new StreamWriter(xmlFilePath))
            {
                stream.Write(serializer);
            }

            _logger.LogInformation("XML file created successfully.");
        }
    }
}