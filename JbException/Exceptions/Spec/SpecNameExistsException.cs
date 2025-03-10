using JbException.Base;
using JbException.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace JbException.Exceptions.Spec
{
    public  class SpecNameExistsException : JbBaseException
    {

        public SpecNameExistsException(string specName) : base($"The spec named {specName} already exists.")
        {
        }

        public SpecNameExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }

        public override string ErrorCode => "SPEC-NAME-EXISTS";


    }
}
