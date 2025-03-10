using JbException.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace JbException.Exceptions.SpecDetail
{
    public class SpecDetailNotFoundException : JbBaseException
    {
        public SpecDetailNotFoundException(string specDetailId) : base($"The spec detail Id {specDetailId} not found.")
        {
        }

        public SpecDetailNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }

        public override string ErrorCode => "SPEC-DETAIL-NOT-FOUND";
    }
}
