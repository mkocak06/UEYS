using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models.ConfigModels
{
    public class YOKSettings
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string EgitimBilgisiUrl { get; set; }
        public string MezunUrl { get; set; }
        public string SaglikMezunUrl { get; set; }
        public string AkademikIdariUrl { get; set; }
        public string AkademisyenGorevlendirmeUrl { get; set; }
        public string AkademisyenUzmanlikUrl { get; set; }
        public string IdariPersonelUrl { get; set; }
    }
}
