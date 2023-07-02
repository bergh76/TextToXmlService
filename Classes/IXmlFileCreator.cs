using TextToXmlService.Models;

namespace TextToXmlService.Classes;

public interface IXmlFileCreator
{
    void WriteXmlFile(string fileName, People people);
}