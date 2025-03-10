using Shared.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.ResponseModels.Student
{
    public class StudentQuotaChartModel
    {
        public QuotaType SeriesName { get; set; }
        public string Name { get; set; }
        //public int EAHCount { get; set; }
        //public int SBACount { get; set; }
        //public int University_StateCount { get; set; }
        //public int University_PrivateCount { get; set; }
        //public int KKTCFullTimeCount { get; set; }
        //public int KKTCHalfTimeCount { get; set; }
        //public int JGKCount { get; set; }
        //public int KKKCount { get; set; }
        //public int HKKCount { get; set; }
        //public int DKKCount { get; set; }
        //public int VetCount { get; set; }
        //public int ChemistCount { get; set; }
        //public int PharmacistCount { get; set; }
        public int Count { get; set; }
    }
}
