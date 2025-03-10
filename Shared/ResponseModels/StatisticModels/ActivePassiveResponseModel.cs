using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.ResponseModels.StatisticModels
{
    public class ActivePassiveResponseModel
    {
        public string SeriesName { get; set; }
        public int ActiveRecordsCount { get; set; }
        public int PassiveRecordsCount { get; set; }
    }
}
