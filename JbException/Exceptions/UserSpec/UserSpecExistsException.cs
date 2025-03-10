using JbException.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace JbException.Exceptions.UserSpec
{
    public class UserSpecExistsException : JbBaseException
    {
        public UserSpecExistsException(string userId, string specDetailName) : base($"The user with {userId} id already has {specDetailName} spec")
        {
        }

        public UserSpecExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }

        public override string ErrorCode => "USER-SPEC-EXISTS";
    }
}
