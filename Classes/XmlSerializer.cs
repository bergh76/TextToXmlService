

using System.Runtime.CompilerServices;

namespace TextToXmlService.Classes
{
    internal class XmlSerializer : IStringSerializer
    {

        /// <summary>
        /// Async - Deserialize string to object of type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="text"></param>
        /// <returns>An object of type T</returns>
        public Task<T> DeserializeAsync<T>(string text)
        {
            var serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));

            using var stream = new StringReader(text);
            var result = Task.FromResult((T)serializer.Deserialize(stream));

            return result;
        }

        /// <summary>
        /// Deserialize string to object of type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="text"></param>
        /// <returns>An object of type T</returns>
        public object Deserialize<T>(string text)
        {
            var serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));

            using var stream = new StringReader(text);
            var result = (T)serializer.Deserialize(stream);

            return result;
        }

        /// <summary>
        /// Async - Serialize an object to Utf8 stream
        /// </summary>
        /// <param name="data"></param>
        /// <returns><c>stream</c> as utf8.</returns>
        public async Task<string> SerializeAsync(object data)
        {
            var serializer = new System.Xml.Serialization.XmlSerializer(data.GetType());

            await using var stream = new Utf8StringWriter();
            serializer.Serialize(stream, data);

            return stream.ToString();
        }

        /// <summary>
        /// Serialize an object to Utf8 stream
        /// </summary>
        /// <param name="data"></param>
        /// <returns><c>stream</c> as utf8.</returns>
        public string Serialize<T>(object data)
        {
            var serializer = new System.Xml.Serialization.XmlSerializer(data.GetType());

            using var stream = new Utf8StringWriter();
            serializer.Serialize(stream, data);

            return stream.ToString();
        }

        
    }
}
