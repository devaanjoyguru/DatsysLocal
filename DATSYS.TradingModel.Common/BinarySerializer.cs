using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;

namespace DATSYS.TradingModel.Common
{
    public class BinarySerializer
    {
        //Serialize object to binary
        public byte[] Serialize(object obj)
        {
            var binaryFormatter = new BinaryFormatter();
            var ms = new MemoryStream();
            binaryFormatter.Serialize(ms, obj);
            return ms.GetBuffer();
        }

        public T Deserialize<T>(byte[] obj)
        {
            var binaryFormatter = new BinaryFormatter();
            var ms = new MemoryStream(obj);
            return (T)binaryFormatter.Deserialize(ms);
        }


    }

    public class StringSerializer
    {
        public string Serialize(object obj)
        {
            var serializer = new XmlSerializer(obj.GetType());
            var ms = new MemoryStream();
            serializer.Serialize(ms,obj);
            ms.Position = 0;
            var reader=new StreamReader(ms);
            var serialized = reader.ReadToEnd();

            return serialized;
        }

        public T DeSerialize<T>(string serialized)
        {
            var serializer = new XmlSerializer(typeof (T));
            TextReader reader=new StringReader(serialized);
            var deserialized= serializer.Deserialize(reader);

            return (T)deserialized;
        }
    }
}
