using JbException.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace JbException.Exceptions.TeamUser
{
    public class UserNotTeamMemberException : JbBaseException
    {
        public UserNotTeamMemberException(string teamName, string userId) : base($"{userId} user is not a member of {teamName}")
        {
        }

        public UserNotTeamMemberException(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }

        public override string ErrorCode => "USER-NOT-TEAM-MEMBER";
    }
}
