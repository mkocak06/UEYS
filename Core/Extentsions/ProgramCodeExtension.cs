using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Extentsions
{
    public static class ProgramCodeExtension
    {
        public static string ConcatenateIntegers(this int provinceCode, int hospitalCode, int professionCode, int expertisebranchCode)
        {
            string formattedValue1 = provinceCode.ToString("D2");
            string formattedValue2 = hospitalCode.ToString("D3");
            string formattedValue3 = professionCode.ToString("D1");
            string formattedValue4 = expertisebranchCode.ToString("D3");

            return $"{formattedValue1}{formattedValue2}{formattedValue3}{formattedValue4}";
        }
    }
}
