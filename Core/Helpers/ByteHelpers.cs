using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Helpers
{
    public class ByteHelpers
    {
        public static byte[] ReadAllBytes(Stream instream)
        {
            if (instream is MemoryStream)
                return ((MemoryStream)instream).ToArray();

            var memoryStream = new MemoryStream();

            instream.CopyTo(memoryStream);
            return memoryStream.ToArray();
        }
    }
}
