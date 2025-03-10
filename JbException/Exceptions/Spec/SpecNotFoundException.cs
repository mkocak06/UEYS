using JbException.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace JbException.Exceptions.Spec
{
    public  class SpecNotFoundException : JbBaseException
    {
        public SpecNotFoundException(string specId) : base($"The spec Id {specId} not found.")
        {
        }

        public SpecNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }

        public override string ErrorCode => "SPEC-NOT-FOUND";
    }
}
