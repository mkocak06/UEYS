using JbException.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace JbException.Exceptions.UserLifeRequest
{
    public  class NotUserActiveRequestException : JbBaseException
    {
        public NotUserActiveRequestException(string userId) : base($"The user {userId}  not active request")
        {
        }

        public NotUserActiveRequestException(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }

        public override string ErrorCode => "USER-NOT-ACTIVE-REQUEST";
    }
}
