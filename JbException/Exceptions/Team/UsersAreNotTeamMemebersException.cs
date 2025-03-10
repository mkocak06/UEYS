using JbException.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace JbException.Exceptions.Team
{
    public class UsersAreNotTeamMemebersException : JbBaseException
    {
        public UsersAreNotTeamMemebersException(string userId, string toUserId) : base($"{userId} and {toUserId} are not members of the same team.")
        {
        }

        public UsersAreNotTeamMemebersException(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }

        public override string ErrorCode => "USERS-NOT-TEAM-MEMBER";
    }
}
