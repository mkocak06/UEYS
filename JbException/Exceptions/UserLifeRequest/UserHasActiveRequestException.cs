using JbException.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace JbException.Exceptions.UserLifeRequest
{
    public class UserHasActiveRequestException : JbBaseException
    {
        public UserHasActiveRequestException(string userId) : base($"The user {userId} has active request")
        {
        }

        public UserHasActiveRequestException(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }

        public override string ErrorCode => "USER-ACTIVE-REQUEST";
    }
}
