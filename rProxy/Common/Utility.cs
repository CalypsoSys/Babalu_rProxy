using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml;
using System.IO;

namespace Babalu.rProxy
{
    /// <summary>
    /// 
    /// </summary>
    public static class Utility
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dispose"></param>
        public static void SafeDispose(IDisposable dispose)
        {
            try
            {
                if (dispose != null)
                    dispose.Dispose();
            }
            catch (Exception excp)
            {
                LogFactory.LogException(excp, "Diposing a object");
            }
        }

        /// <summary>
        /// look for a match in the raw data
        /// </summary>
        /// <param name="input">the data to interogate</param>
        /// <param name="index">the index to start the search</param>
        /// <param name="match">what to match on</param>
        /// <returns></returns>
        public static bool Match(List<byte> input, int index, string match)
        {
            // search for the match token starting at this location
            for (int x = 0; x < match.Length; x++)
            {
                if (input.Count <= (index + x) || input[index + x] != match[x])
                    return false;
            }

            return true;
        }

        /// <summary>
        /// decode base 64 string
        /// </summary>
        /// <param name="encodedData"></param>
        /// <returns></returns>
        public static string DecodeFrom64(string encodedData)
        {
            byte[] encodedDataAsBytes = Convert.FromBase64String(encodedData);

            return Encoding.ASCII.GetString(encodedDataAsBytes);
        }


        /// <summary>
        /// Used to de-serialize an wcf object from xml string format.
        /// </summary>
        /// <param name="objectType">the type of object to deserialize</param>
        /// <param name="serializedObject">the xml string version of the object you want to deserialize.</param>
        /// <returns>the object you deserialized.</returns>
        public static object WcfDeserialize(Type objectType, string serializedObject)
        {
            if (serializedObject != null)
            {
                DataContractSerializer serializer = new DataContractSerializer(objectType);
                using (StringReader reader = new StringReader(serializedObject))
                {
                    XmlReaderSettings settings = new XmlReaderSettings();
                    settings.CheckCharacters = false;

                    using (XmlReader xmlReader = XmlReader.Create(reader, settings))
                        return serializer.ReadObject(xmlReader);
                }
            }
            else
                return null;
        }

        /// <summary>
        /// Used to serialize an object into a wcf xml format.
        /// </summary>
        /// <param name="objectToSerialize">the object you want to serialize</param>
        /// <returns>a xml string.</returns>
        public static string WcfSerialize(object objectToSerialize)
        {
            if (objectToSerialize != null)
            {
                DataContractSerializer serializer = new DataContractSerializer(objectToSerialize.GetType());
                using (StringWriter writer = new StringWriter())
                {
                    XmlWriterSettings settings = new XmlWriterSettings();
                    settings.CheckCharacters = false;

                    using (XmlWriter xmlWriter = XmlWriter.Create(writer, settings))
                    {
                        serializer.WriteObject(xmlWriter, objectToSerialize);
                        xmlWriter.Flush();
                        return writer.ToString();
                    }
                }
            }
            else
                return null;
        }
    }
}
