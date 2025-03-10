using JbException.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace JbException.Exceptions.User
{
    public  class UserNotFoundException : JbBaseException
    {
        public UserNotFoundException(string userId) : base($"The user Id {userId} not found.")
        {
        }

        public UserNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }

        public override string ErrorCode => "USER-NOT-FOUND";
    }
}
