using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Helpers
{
    public class StringHelpers
    {
        public static string MaskIdentityNumber(string idNumber)
        {
            return string.Concat(idNumber.AsSpan(0, 3), new string('*', 6), idNumber.AsSpan(9, 2));
        }
    }
}
