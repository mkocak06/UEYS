using JbException.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace JbException.Exceptions.UserSpec
{
    public  class UserSpecNotFoundException: JbBaseException
    {
        public UserSpecNotFoundException(string userId) : base($"No suitable spec for {userId} could be found.")
        {
        }

        public UserSpecNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }

        public override string ErrorCode => "USER-SPEC-NOT-FOUND";
    }
}
