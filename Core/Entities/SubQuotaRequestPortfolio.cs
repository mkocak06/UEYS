using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class SubQuotaRequestPortfolio : BaseEntity
    {
        public int? Answer { get; set; }
        public long? PortfolioId { get; set; }
        public Portfolio Portfolio { get; set; }
        public long? SubQuotaRequestId { get; set; }
        public SubQuotaRequest SubQuotaRequest { get; set; }

    }
}
