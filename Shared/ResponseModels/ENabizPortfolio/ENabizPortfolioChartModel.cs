using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.ResponseModels.ENabizPortfolio
{
    public class ENabizPortfolioChartModel
    {
        public DateTime? IslemTarihiDate { get; set; }
        public string KlinikAdi { get; set; }
        public int? IslemSayisi { get; set; }
        public int? MuayeneSayisi { get; set; }
        public int? AmeliyatSayisi { get; set; }
        public string IslemTarihi
        {
            get
            {
                return IslemTarihiDate?.ToString("MMMM yyyy", new CultureInfo("tr-TR"));
            }
        }
    }
}
