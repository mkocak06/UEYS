using Shared.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.BaseModels;

namespace Shared.ResponseModels
{
    public class UpdateUserAccountInfoResponseDTO : UpdateUserAccountInfoBase
    {
        public long Id { get; set; }
    }
}
