using JbException.Base;
using JbException.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace JbException.Exceptions
{
    public  class EmptyApiRequestException : JbBaseException
    {
        public EmptyApiRequestException(string paramName) : this(paramName, null)
        {
        }

        public EmptyApiRequestException(string paramName, string message) : base(string.IsNullOrEmpty(message) ? $"Api request object '{paramName}' is null or empty." : $"{message}. ParamName: {paramName}")
        {
        }

        public override string ErrorCode => "REQ-OBJ-NULL-EMPTY";
        public override ExceptionLevel ExceptionLevel => ExceptionLevel.Domain;

        public EmptyApiRequestException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
