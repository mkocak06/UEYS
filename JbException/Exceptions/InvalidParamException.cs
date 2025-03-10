using JbException.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace JbException.Exceptions
{
    public  class InvalidParamException : JbBaseException
    {
        public InvalidParamException(string message) : base(message)
        {
        }


        public override string ErrorCode => "INVALID-PARAM";

        public InvalidParamException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
