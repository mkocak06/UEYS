using Shared.FilterModels.Base;
using Shared.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.FilterModels
{
    public class UserFilterDTO : BaseFilter
    {
        public string UserId { get; set; }
        public string NickName { get; set; }
        public string Email { get; set; }
        public string CountryCode { get; set; }
        public string Language { get; set; }

    }
}
