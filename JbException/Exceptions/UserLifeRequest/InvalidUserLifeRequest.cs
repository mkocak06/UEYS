using JbException.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace JbException.Exceptions.UserLifeRequest
{
    public  class InvalidUserLifeRequest : JbBaseException
    {
        public InvalidUserLifeRequest(string userLifeRequestId) : base($"The user life request {userLifeRequestId}  not invalid")
        {
        }

        public InvalidUserLifeRequest(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }

        public override string ErrorCode => "INVALID-USER_LIFE-REQUEST";
    }
}
