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
        if (inputPath == null)
        {
            _logger.LogInformation("Input path from appsettings.json is empty");
            return;
        }

        var fileNames = Directory.GetFiles(inputPath);

        if (!fileNames.Any())
        {
            _logger.LogInformation("No file to handle....");
            return;
        }

        foreach (var fileName in fileNames)
        {
            _logger.LogInformation("Handle File: {FileName}", fileName);
            var filePath = Path.Combine(inputPath, fileName);
            var xmlFileName = $"{Path.GetFileNameWithoutExtension(filePath)}_{DateTime.Today:yyyy_MM_dd}.xml";
            if (_filePathOptions.Output == null)
            {
                _logger.LogInformation("Output path from appsettings.json is empty");

                return;
            }

            var xmlFilePath = Path.Combine(_filePathOptions.Output, xmlFileName);
            var lines = File.ReadAllLines(filePath);

            // Create the People object
            var people = new People();
            _logger.LogInformation("Instantiate new People: {@People}", people);
            people.Person = new List<Person>();

            // Initialize variables to hold person and family information
            Person? currentPerson = null;
            Family? currentFamily = null;

            // Iterate over lines and create People, Person, and Family objects
            foreach (var line in lines)
            {
                var parts = line.Split('|');
                var rowType = parts[0];

                try
                {
                    switch (rowType)
                    {
                        case "P":
                            newPerson = new Person();
                            newPerson.Firstname = lineParts.Length > 1 ? lineParts[1].Trim() : null;
                            newPerson.Lastname = lineParts.Length > 2 ? lineParts[2].Trim() : null;
                            newPerson.Address = new Address();
                            newPerson.Phone = new Phone();
                            newPerson.Family = new List<Family>();

                            _logger.LogInformation("Adding Person: {@Person} to People: {@People}", newPerson, people);

                            newFamily = null;
                            people.Person.Add(newPerson);

                            break;
                        case "T":
                            if (newFamily == null)
                            {
                                if (newPerson is { Phone: not null })
                                {
                                    _logger.LogInformation("Adding Phone: {@Phone} for Person: {@Person}",
                                        newPerson.Phone, newPerson);
                                    newPerson.Phone.Mobile = lineParts.Length > 1 ? lineParts[1].Trim() : null;
                                    newPerson.Phone.Landline = lineParts.Length > 2 ? lineParts[2].Trim() : null;
                                }
                            }
                            else
                            {
                                if (newFamily.Phone != null)
                                {
                                    _logger.LogInformation("Adding Phone: {@Phone} for Family: {@Family}",
                                        newFamily.Phone, newFamily);
                                    newFamily.Phone.Mobile = lineParts.Length > 1 ? lineParts[1].Trim() : null;
                                    newFamily.Phone.Landline = lineParts.Length > 2 ? lineParts[2].Trim() : null;
                                }
                            }

                            break;
                        case "A":
                            if (newFamily == null)
                            {
                                if (newPerson is { Address: not null })
                                {
                                    _logger.LogInformation("Adding Address: {@Address} for Person: {@Person}",
                                        newPerson.Address, newPerson);
                                    newPerson.Address.Street = lineParts.Length > 1 ? lineParts[1].Trim() : null;
                                    newPerson.Address.City = lineParts.Length > 2 ? lineParts[2].Trim() : null;
                                    newPerson.Address.Zipcode = lineParts.Length > 3 ? lineParts[3].Trim() : null;
                                }
                            }
                            else
                            {
                                if (newFamily.Address != null)
                                {
                                    _logger.LogInformation("Adding Address: {@Address} for Family: {@Family}",
                                        newFamily.Address, newFamily);
                                    newFamily.Address.Street = lineParts.Length > 1 ? lineParts[1].Trim() : null;
                                    newFamily.Address.City = lineParts.Length > 2 ? lineParts[2].Trim() : null;
                                    newFamily.Address.Zipcode = lineParts.Length > 3 ? lineParts[3].Trim() : null;
                                }
                            }

                            break;
                        case "F":
                            newFamily = new Family();
                            _logger.LogInformation("Adding Family: {@Family}", newFamily);
                            newFamily.Name = lineParts.Length > 1 ? lineParts[1].Trim() : null;
                            newFamily.Born = lineParts.Length > 2 ? lineParts[2].Trim() : null;
                            newFamily.Address = new Address();
                            newFamily.Phone = new Phone();
                            newPerson?.Family.Add(newFamily);
                            _logger.LogInformation("Adding Family {@Family} to Person: {@Person}", newFamily,
                                newPerson);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError("Can not read out line {@Line}. {NewLine}Exception:{@Ex}", line,
                        Environment.NewLine, ex);
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