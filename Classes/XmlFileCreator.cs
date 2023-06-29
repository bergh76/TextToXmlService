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

    public void WriteXmlFile()
    {
        _logger.LogInformation("Start creating XML");
        var inputPath = _filePathOptions.Input;

        var fileNames = Directory.GetFiles(inputPath);

        if (!fileNames.Any())
            _logger.LogInformation("No file to handle....");

        foreach (var fileName in fileNames)
        {
            _logger.LogInformation("Handle File: {FileName}", fileName);
            var filePath = Path.Combine(inputPath, fileName);
            var xmlFileName = $"{Path.GetFileNameWithoutExtension(filePath)}_{DateTime.Today:yyyy_MM_dd}.xml";
            var xmlFilePath = Path.Combine(_filePathOptions.Output, xmlFileName);
            var lines = File.ReadAllLines(filePath);

            // Create the People object
            var people = new People();
            people.Person = new List<Person>();
            _logger.LogInformation("Instantiate new Person {People}", people);

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
                        _logger.LogInformation("Instantiate new Person {CurrentPerson}", currentPerson);
                        currentFamily = null;
                        people.Person.Add(currentPerson);

                        break;
                    case "T":
                        if (currentFamily == null)
                        {
                            if (currentPerson is { Phone: not null })
                            {
                                currentPerson.Phone.Mobile = parts.Length > 1
                                    ? parts[1].Trim()
                                    : null;
                                currentPerson.Phone.Landline = parts.Length > 2
                                    ? parts[2].Trim()
                                    : null;
                            }
                        }
                        else
                        {
                            if (currentFamily.Phone != null)
                            {
                                currentFamily.Phone.Mobile = parts.Length > 1
                                    ? parts[1].Trim()
                                    : null;
                                currentFamily.Phone.Landline = parts.Length > 2
                                    ? parts[2].Trim()
                                    : null;
                            }
                        }

                        break;
                    case "A":
                        if (currentFamily == null)
                        {
                            if (currentPerson is { Address: not null })
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
                        }
                        else
                        {
                            if (currentFamily.Address != null)
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

            _logger.LogInformation("People as collection {People}", people.Person);
            // Serialize the People object to XML
            var serializer = _stringSerializer.Serialize<People>(people);
            using (var stream = new StreamWriter(xmlFilePath))
            {
                _logger.LogInformation("Writing file {FileName}", xmlFileName);
                stream.Write(serializer);
            }

            _logger.LogInformation("XML file created successfully. {FileName}", xmlFileName);
        }
    }
}