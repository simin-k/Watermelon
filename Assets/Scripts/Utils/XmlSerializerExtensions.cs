using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Wartermelon.Utils
{
    public static class XmlUtility
    {
        #region Private fields
        private static readonly Dictionary<RuntimeTypeHandle, XmlSerializer> MS_serializers = new Dictionary<RuntimeTypeHandle, XmlSerializer>();
        #endregion
        #region Public methods
        /// <summary>
        ///   Serialize object to xml string by <see cref = "XmlSerializer" />
        /// </summary>
        /// <typeparam name = "T"></typeparam>
        /// <param name = "value"></param>
        /// <returns></returns>
        public static string ToXml<T>(T value, Formatting formatting = Formatting.None)
            where T : new()
        {
            var serializer = GetValue(typeof(T));
            using (var stream = new MemoryStream())
            {
                using (var writer = new XmlTextWriter(stream, new UTF8Encoding()))
                {
                    writer.Formatting = formatting;
                    serializer.Serialize(writer, value);
                    return Encoding.UTF8.GetString(stream.ToArray());
                }
            }
        }
        /// <summary>
        ///   Serialize object to stream by <see cref = "XmlSerializer" />
        /// </summary>
        /// <typeparam name = "T"></typeparam>
        /// <param name = "value"></param>
        /// <param name = "stream"></param>
        public static void ToXml<T>(T value, Stream stream)
            where T : new()
        {
            var serializer = GetValue(typeof(T));
            serializer.Serialize(stream, value);
        }

        /// <summary>
        ///   Deserialize object from string
        /// </summary>
        /// <typeparam name = "T">Type of deserialized object</typeparam>
        /// <param name = "srcString">Xml source</param>
        /// <returns></returns>
        public static T FromXml<T>(string srcString)
            where T : new()
        {
            var serializer = GetValue(typeof(T));
            using (var stringReader = new StringReader(srcString))
            {
                using (XmlReader reader = new XmlTextReader(stringReader))
                {
                    return (T)serializer.Deserialize(reader);
                }
            }
        }
        /// <summary>
        ///   Deserialize object from stream
        /// </summary>
        /// <typeparam name = "T">Type of deserialized object</typeparam>
        /// <param name = "source">Xml source</param>
        /// <returns></returns>
        public static T FromXml<T>(Stream source)
            where T : new()
        {
            var serializer = GetValue(typeof(T));
            return (T)serializer.Deserialize(source);
        }
        #endregion
        #region Private methods
        private static XmlSerializer GetValue(Type type)
        {
            if (!MS_serializers.TryGetValue(type.TypeHandle, out var serializer))
            {
                lock (MS_serializers)
                {
                    if (!MS_serializers.TryGetValue(type.TypeHandle, out serializer))
                    {
                        serializer = new XmlSerializer(type);

                        MS_serializers.Add(type.TypeHandle, serializer);
                    }
                }
            }
            return serializer;
        }
        #endregion
    }

}

