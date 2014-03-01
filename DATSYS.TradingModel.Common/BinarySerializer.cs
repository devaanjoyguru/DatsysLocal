using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

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
}
