using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Options;
using TextToXmlService.Models;

namespace TextToXmlService.Classes;

public class TextFileHelper : ITextFileHelper
{
    private readonly ILogger<XmlFileCreator> _logger;
    private readonly FilePathOptions _filePathOptions;

    public TextFileHelper(ILogger<XmlFileCreator> logger, IOptions<FilePathOptions> filePathOptions)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _filePathOptions = filePathOptions.Value ?? throw new ArgumentNullException(nameof(filePathOptions));
    }


    /// <summary>
    /// Get all files in input folder
    /// </summary>
    /// <returns></returns>
    public List<string> GetFiles()
    {
        _logger.LogInformation("Get files in input folder.");

        var inputPath = _filePathOptions.Input;
        if (inputPath == null)
        {
            _logger.LogInformation("Input path from appsettings.json is empty");
            return new List<string>();
        }

        var fileNames = Directory.GetFiles(inputPath);

        return fileNames.Any() ? fileNames.ToList() : new List<string>();
    }

    /// <summary>
    /// Create folders for files.
    /// </summary>
    /// <returns></returns>
    public Dictionary<string, bool> CreateFolders()
    {
        var foldersExists = new Dictionary<string, bool>();
        var inputPath = _filePathOptions.Input;
        if (inputPath == null)
        {
            foldersExists.Add("Can not create input folder", false);
        }
        else
        {
            if (!Directory.Exists(inputPath))
            {
                Directory.CreateDirectory(inputPath);
                foldersExists.Add("Input folder created.", true);
            }
            else
            {
                foldersExists.Add("Input folder exists.", true);
            }
        }

        var outputPah = _filePathOptions.Output;
        if (outputPah == null)
        {
            foldersExists.Add("Can not create output folder.", false);
        }
        else
        {
            if (!Directory.Exists(outputPah))
            {
                Directory.CreateDirectory(outputPah);
                foldersExists.Add("Output folder created.", true);
            }
            else
            {
                foldersExists.Add("Output folder exists.", true);
            }
        }

        var archivePath = _filePathOptions.Archive;
        if (archivePath == null)
        {
            foldersExists.Add("Can not create archive folder.", false);
        }
        else
        {
            if (!Directory.Exists(archivePath))
            {
                Directory.CreateDirectory(archivePath);
                foldersExists.Add("Archive folder created.", true);
            }
            else
            {
                foldersExists.Add("Archive folder exists.", true);
            }
        }

        return foldersExists;
    }

    /// <summary>
    /// Move file to archive when handled.
    /// </summary>
    /// <param name="fileName"></param>
    public void MoveFile(string fileName)
    {
        var inputPath = _filePathOptions.Input != null
            ? Path.Combine(_filePathOptions.Input, fileName)
            : null;
        if (inputPath == null)
        {
            _logger.LogInformation("Missing input path in appsetting.json.");
            return;
        }

        var file = Path.GetFileName(fileName);
        var archivePath = _filePathOptions.Archive != null
            ? Path.Combine(_filePathOptions.Archive, file)
            : null;
        if (archivePath == null)
        {
            _logger.LogInformation("Missing archive path in appsetting.json.");
            return;
        }

        if (!File.Exists(inputPath))
        {
            _logger.LogInformation("Can not move file {InputPath}. The file does not exist.", inputPath);
            return;
        }

        _logger.LogInformation("Moving file {inputPath}", inputPath);
        if (File.Exists(archivePath))
        {
            file = Path.GetFileNameWithoutExtension(archivePath);
            var fileExt = Path.GetExtension(archivePath);
            archivePath = Path.Combine(_filePathOptions.Archive, $"{file}_{Path.GetRandomFileName()}{fileExt}");
        }
        File.Move(inputPath, archivePath);

        _logger.LogInformation("File {InputPath} is moved to {ArchivePath}", inputPath, archivePath);
    }

    /// <summary>
    /// Read the text file content and return a People object.
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns>Returns a <c>People</c> object.</returns>
    public People? ReadTextFile(string fileName)
    {
        _logger.LogInformation("Start reading file: {FileName}", fileName);

        var inputPath = _filePathOptions.Input;
        if (inputPath == null)
        {
            _logger.LogInformation("Input path from appsettings.json is empty");
            return null;
        }

        var filePath = Path.Combine(inputPath, fileName);
        var lines = File.ReadAllLines(filePath);

        // Create the People object
        var people = new People();
        _logger.LogInformation("Instantiate new People: {@People}", people);
        people.Person = new List<Person>();

        // Initialize person and family
        Person? newPerson = null;
        Family? newFamily = null;

        foreach (var line in lines)
        {
            var lineParts = line.Split('|');
            var rowType = lineParts[0];

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

                        _logger.LogInformation("Adding Person: {@Person} to People: {@People}", newPerson,
                            people);

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

        return people;
    }
}