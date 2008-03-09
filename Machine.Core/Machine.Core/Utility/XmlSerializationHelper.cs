using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;

namespace Machine.Core.Utility
{
  public class XmlSerializationHelper
  {
    public static string Serialize<T>(T obj)
    {
      XmlSerializer serializer = new XmlSerializer(typeof(T));
      StringWriter writer = new StringWriter();
      serializer.Serialize(writer, obj);
      return writer.ToString();
    }

    public static T DeserializeString<T>(string value)
    {
      XmlSerializer serializer = new XmlSerializer(typeof(T));
      return (T)serializer.Deserialize(new StringReader(value));
    }
  }
}
