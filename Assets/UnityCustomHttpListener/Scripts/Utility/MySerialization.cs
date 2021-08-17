using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace UnityCustomHttpListener.Scripts.Utility
{
    public static class MySerialization
    {
        // https://stackoverflow.com/questions/1446547/how-to-convert-an-object-to-a-byte-array-in-c-sharp
        public static byte[] ObjectToByteArray(this object obj)
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
            
        }

        public static byte[] StringToByteArray(this string str)
        {
            return System.Text.Encoding.UTF8.GetBytes(str);
        }
    }
}