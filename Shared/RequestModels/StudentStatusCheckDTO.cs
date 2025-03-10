using Shared.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.RequestModels
{
    public class StudentStatusCheckDTO
    {
        public bool? IsTransferNecessary { get; set; }
        public bool? IsEndOfEducationNecessary { get; set; }
    }
}
