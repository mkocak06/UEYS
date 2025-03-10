using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IXmlService
    {
        string ToSerialize<T>(object obj);
        T ToDeserialize<T>(string xml);
    }
}
