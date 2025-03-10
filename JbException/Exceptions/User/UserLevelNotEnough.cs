using JbException.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace JbException.Exceptions.User
{
    public class UserLevelNotEnough : JbBaseException
    {
        public UserLevelNotEnough(string userId) : base($"The user Id {userId} level information is not enough.")
        {
        }

        public UserLevelNotEnough(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }

        public override string ErrorCode => "USER-LEVELT-NOT-ENOUGH";
    }
}
