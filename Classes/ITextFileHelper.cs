using TextToXmlService.Models;

namespace TextToXmlService.Classes;

public interface ITextFileHelper
{
    /// <summary>
    /// Get all files in input folder
    /// </summary>
    /// <returns></returns>
    List<string> GetFiles();

    /// <summary>
    /// Read the text file content and return a People object.
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns>Returns a <c>People</c> object.</returns>
    People? ReadTextFile(string fileName);


    Dictionary<string, bool> CreateFolders();

    /// <summary>
    /// Move file to archive when handled.
    /// </summary>
    /// <param name="fileName"></param>
    void MoveFile(string fileName);
}