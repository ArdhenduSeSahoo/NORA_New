using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
//using System.Web.Script.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace Damco.Model
{
    /// <summary>
    /// Serializes data to JSON string or XML and back. 
    /// </summary>
    /// <remarks>
    /// Uses JavaScriptSerializer for JSON and XMLSerializer for XML.
    /// </remarks>
    public static class Serialization
    {
        //private class FixedJsonConverter : System.Web.Script.Serialization.JavaScriptConverter
        //{
        //    public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        //    {

        //        var result = new DynamicEntity();
        //        result.PropertyMapping = new Dictionary<string, int>();
        //        result.Values = new object[dictionary.Count];
        //        var i = 0;
        //        foreach (var item in dictionary)
        //        {
        //            result.PropertyMapping[item.Key] = i;
        //            result.Values[i] = item.Value;
        //            i++;
        //        }
        //        return result;
        //    }

        //    public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        //    {
        //        var dynamic = ((DynamicEntity)obj);
        //        return dynamic.PropertyMapping.ToDictionary(p => p.Key, p => dynamic.GetValue(p.Value));
        //    }

        //    public override IEnumerable<Type> SupportedTypes { get { return new[] { typeof(DynamicEntity) }; } }
        //}

        //static JavaScriptSerializer _serializer = new JavaScriptSerializer();
        static Serialization()
        {
            //_serializer = new JavaScriptSerializer();
            //_serializer.RegisterConverters(new[] { new FixedJsonConverter() });
        }
        /// <summary>
        /// Converts an object to a JSON string.
        /// </summary>
        /// <typeparam name="T">The type of the source object.</typeparam>
        /// <param name="value">Object to be serialized.</param>
        /// <returns>Serialized string.</returns>
        public static string? ToJson<T>(this T value)
        {
            //return _serializer.Serialize(value);
            return JsonConvert.SerializeObject(value);
        }

        /// <summary>
        /// Converts the specified JSON string to an object of type T.
        /// </summary>
        /// <typeparam name="T">The type of the resulting object</typeparam>
        /// <param name="json">JSON string to be deserialized</param>
        /// <returns>Deserialized object</returns>
        public static T? FromJson<T>(this string json)
        {
            if (json == null)
                return default;
            else
                return JsonConvert.DeserializeObject<T>(json);//_serializer.Deserialize<T>(json);
        }

        /// <summary>
        /// Converts a JSON formatted string to an object of the specified type.
        /// </summary>
        /// <param name="json">JSON string to be deserialized</param>
        /// <param name="targetType">The type of the resulting object</param>
        /// <returns>Deserialized object</returns>
        public static object? FromJson(this string json, Type targetType)
        {
            if (json == null)
            {
                if (targetType.IsClass)
                    return null;
                else
                    return Activator.CreateInstance(targetType);
            }
            else
                return JsonConvert.DeserializeObject(json, targetType); //_serializer.Deserialize(json, targetType);
        }

        private static ConcurrentDictionary<Tuple<Type, string, string>, XmlSerializer> sdicSerializerCache = new ConcurrentDictionary<Tuple<Type, string, string>, XmlSerializer>();

        private static XmlSerializer GetXmlSerializer(Type type, string namespaceUri, string localName)
        {
            var key = Tuple.Create(type, namespaceUri, localName);
            return sdicSerializerCache.GetOrAdd(key, k =>
            {
                return CreateXmlSerializer(k.Item1, k.Item2, k.Item3);
            });
        }
        private static XmlSerializer? CreateXmlSerializer(Type type, string namespaceUri, string localName)
        {
            //Find a better way to do the next if (it won't work well for collections)
            //We need the if because using an XmlRootAttribute makes it very slow.
            XmlSerializer xmlser;
            if ((namespaceUri ?? "") == "" && (localName == type.Name || (localName ?? "") == ""))
                xmlser = new XmlSerializer(type);
            else
            {
                //We need to add an XmlRootAttribute to let it deal with the differences between
                //the standard serialization and what the reader contains.
                XmlRootAttribute xmlra = new XmlRootAttribute(localName);
                xmlra.Namespace = namespaceUri;
                xmlser = new XmlSerializer(type, xmlra);
            }
            return xmlser;
        }

        private static object? FromXml(System.Xml.XmlReader reader, Type type)
        {
            XmlSerializer xmlser = GetXmlSerializer(type, reader.NamespaceURI, reader.LocalName);
            try
            {
                return xmlser.Deserialize(reader);
            }
            catch (InvalidOperationException ex)
            {
                //An error in the XML document.
                //If it has an innerexception, that will have the real error message.
                if (ex.InnerException != null)
                    throw ex.InnerException;
                else
                    throw;
            }
        }

        /// <summary>
        /// Converts a XML string to target type.
        /// </summary>
        /// <param name="xml">The XML string to be deserialized.</param>
        /// <param name="targetType">The type of the resulting object.</param>
        /// <returns>Deserialized object.</returns>
        public static object FromXml(this string xml, Type targetType)
        {
            XmlReader xmlr;
            if (xml.TrimStart().StartsWith(@"<?")) //Document
                xmlr = new XmlTextReader(xml, XmlNodeType.Document, null);
            else
            {
                xmlr = new XmlTextReader(xml, XmlNodeType.Element, null);
                xmlr.Read(); //Have to read once otherwise it does not work correctly
            }
            return FromXml(xmlr, targetType);
        }

        /// <summary>
        /// Converts the specified XML string to an object of type T.
        /// </summary>
        /// <typeparam name="T">The type of the resulting object.</typeparam>
        /// <param name="xml">The XML string to be deserialized.</param>
        /// <returns>Deserialized object.</returns>
        public static T? FromXml<T>(this string xml)
        {
            return (T)FromXml(xml, typeof(T));
        }

        /// <summary>
        /// Serializes source object to XML documents of type T.
        /// </summary>
        /// <typeparam name="T">The type of the source object.</typeparam>
        /// <param name="value">Source file from which XML document is to be created.</param>
        /// <returns></returns>
        public static string? ToXml<T>(this T value) => ToXml(source: value, typeof(T));

        /// <summary>
        /// Serializes source object to XML documents of type T and checks for Errors.
        /// While errors are to be ignored, if any exception is caught then it returns null.
        /// </summary>
        /// <typeparam name="T">The type of the source object</typeparam>
        /// <param name="value">Source file from which XML document is to be created.</param>
        /// <param name="ignoreErrors">Boolean value - if set to True, returns null when there is any exception. </param>
        /// <returns></returns>
        public static string? ToXml<T>(this T value, bool ignoreErrors)
        {
            if (ignoreErrors)
            {
                try
                {
                    return ToXml<T>(value);
                }
                catch
                {
                    return null;
                }
            }
            else
                return ToXml<T>(value);
        }

        private static string ToXml(object source, Type serializationType)
        {
            XmlSerializer xmlser;
            try
            {
                xmlser = new XmlSerializer(serializationType);
            }
            catch (InvalidOperationException ex)
            {
                Exception exToThrow = ex;
                while (exToThrow.Message != null && (exToThrow.Message.StartsWith("There was an error reflecting") || exToThrow.Message.StartsWith("Cannot serialize")) && exToThrow.InnerException != null)
                    exToThrow = exToThrow.InnerException;
                throw exToThrow;
            }


            StringWriter stwr = new StringWriter();
            try
            {
                xmlser.Serialize(stwr, source);
            }
            catch (InvalidOperationException ex)
            {
                if (ex.Message == "There was an error generating the XML document." && ex.InnerException != null)
                    throw ex.InnerException;
                else
                    throw ex;
            }
            return stwr.ToString();
        }

        /// <summary>
        /// Writes the XML document to a file using the specified XmlWriter.
        /// </summary>
        /// <param name="value">The object to serialize.</param>
        /// <param name="writer">The XmlWriter used to write the XML document.</param>
        /// <param name="includeStartAndFinishElements">Indicates whether to include start and
        /// finish elements of the XML document.
        /// </param>
        public static void WriteXml(object value, System.Xml.XmlWriter writer, bool includeStartAndFinishElements)
        {
            if (includeStartAndFinishElements)
            {
                XmlSerializer xmlser = new XmlSerializer(value.GetType());
                xmlser.Serialize(writer, value);
            }
            else //Remove the root node
            {
                //First we create a string which holds the entire XML.
                XmlSerializer xmlser = new XmlSerializer(value.GetType());
                StringWriter strw = new System.IO.StringWriter();
                xmlser.Serialize(strw, value);
                string strXML = strw.ToString();

                //We could load the string in an XMLNode and use .InnerXml to get
                //Everything within the root node. However, this causes extra namespaces
                //to be written for the nullable types (and maybee other stuff too).
                //Therefore, we manually remove the root node by taking everything from
                //the second end tag to the last begin tag.
                int intStartPos = strXML.IndexOf(@">", strXML.IndexOf(@">") + 1) + 1;
                int intLength = strXML.LastIndexOf(@"</") - intStartPos;
                //We need an XmlDocument to get the root nodes attributes.
                //We will not load the entire XML document in it (it could be rather large)
                //but instead will take just the root node and finish it with /> so
                //it won't need an end node.
                if (intLength < 0) //Happens when there is no end tag
                {
                    XmlDocument xmldoc = new XmlDocument();
                    xmldoc.LoadXml(strXML.Substring(0, intStartPos - 1) + ">");
                    //Copy attributes
                    foreach (XmlAttribute xmla in xmldoc.ChildNodes[1].Attributes)
                        writer.WriteAttributeString(xmla.Name, xmla.Value);
                }
                else
                {
                    XmlDocument xmldoc = new XmlDocument();
                    xmldoc.LoadXml(strXML.Substring(0, intStartPos - 1) + @"/>");
                    //Copy attributes
                    foreach (XmlAttribute xmla in xmldoc.ChildNodes[1].Attributes)
                        writer.WriteAttributeString(xmla.Name, xmla.Value);
                    //Write XML
                    writer.WriteRaw(strXML.Substring(intStartPos, intLength));
                }
            }
        }

    }
}
