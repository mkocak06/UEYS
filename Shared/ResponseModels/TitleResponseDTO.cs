using Shared.BaseModels;
using Shared.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.ResponseModels
{
    public class TitleResponseDTO : TitleBase
    {
        public long? Id { get; set; }
    }
}
