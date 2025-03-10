using JbException.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace JbException.Exceptions.SpecDetail
{
    public class SpecDetailNotActiveException : JbBaseException
    {
        public SpecDetailNotActiveException(string specDetailId) : base($"The spec detail Id {specDetailId} not active.")
        {
        }

        public SpecDetailNotActiveException(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }

        public override string ErrorCode => "SPEC-DETAIL-NOT-ACTIVE";
    }
}
