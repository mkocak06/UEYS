using Core.Interfaces;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Infrastructure.Services
{
    public class XmlService : IXmlService
    {
        public string ToSerialize<T>(object obj)
        {
            XmlSerializer ser = new(typeof(T));
            MemoryStream ms = new();

            ser.Serialize(ms, obj);

            return Encoding.Default.GetString(ms.GetBuffer(), 0, (int)ms.Length);
        }
        public T ToDeserialize<T>(string xml)
        {
            T result;
            XmlSerializer ser = new(typeof(T));
            using (TextReader tr = new StringReader(xml))
            {
                result = (T)ser.Deserialize(tr);
            }
            return result;
        }
    }
}
