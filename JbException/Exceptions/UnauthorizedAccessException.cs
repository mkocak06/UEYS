using JbException.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace JbException.Exceptions
{
    public class UnauthorizedAccessException : JbBaseException
    {
        public UnauthorizedAccessException(string paramName) : base($"You do not have permission to change {paramName}")
        {
        }


        public override string ErrorCode => "UNAUTHORIZED-ACCESS";
     
        public UnauthorizedAccessException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
