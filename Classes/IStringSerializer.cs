namespace TextToXmlService.Classes;

public interface IStringSerializer
{
    /// <summary>
    /// Async - Deserialize string to object of type T
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="text"></param>
    /// <returns>An object of type T</returns>
    Task<T> DeserializeAsync<T>(string text);

    /// <summary>
    /// Deserialize string to object of type T
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="text"></param>
    /// <returns>An object of type T</returns>
    object Deserialize<T>(string text);

    /// <summary>
    /// Async - Serialize an object of type T to Utf8 stream
    /// </summary>
    /// <param name="data"></param>
    /// <returns><c>stream</c> as utf8.</returns>
    Task<string> SerializeAsync<T>(object data);

    /// <summary>
    /// Serialize an object of type T to Utf8 stream
    /// </summary>
    /// <param name="data"></param>
    /// <returns><c>stream</c> as utf8.</returns>
    string Serialize<T>(object data);
}