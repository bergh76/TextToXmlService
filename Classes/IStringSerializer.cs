using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextToXmlService.Classes
{
    public interface IStringSerializer
    {
        /// <summary>
        /// Deserialize string to object of type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="text"></param>
        /// <returns>An object of type T</returns>
        Task<T> DeserializeAsync<T>(string text);

        object Deserialize<T>(string text);

        /// <summary>
        /// Serialize an object to Utf8 stream.
        /// </summary>
        /// <param name="data">Data object as <c>T</c>.</param>
        /// <returns><c>stream</c> as utf8.</returns>
        Task<string> SerializeAsync<T>(object data);

        string Serialize<T>(object data);
    }
}
