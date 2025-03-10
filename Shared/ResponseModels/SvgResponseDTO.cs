using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.BaseModels;

namespace Shared.ResponseModels
{
    public class SvgResponseDTO
    {
        public string Id { get; set; }
        public string Content{ get; set; }
        public int IconType{ get; set; }
    }
}
