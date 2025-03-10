using JbException.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace JbException.Exceptions.TeamUser
{
    public class UserNotFoundTeam:JbBaseException
    {
        public UserNotFoundTeam(string teamName, string userId) : base($"{userId} user not found in {teamName} team")
        {
        }

        public UserNotFoundTeam(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }

        public override string ErrorCode => "USER-NOT-FOUND-TEAM";
    }
}
