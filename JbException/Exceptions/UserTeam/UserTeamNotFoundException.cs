using JbException.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace JbException.Exceptions.UserTeam
{
    public class UserTeamNotFoundException : JbBaseException
    {
        public UserTeamNotFoundException(string userId) : base($"The user {userId} has not been assigned a team")
        {
        }

        public UserTeamNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }

        public override string ErrorCode => "USER-TEAM-NOT-FOUND";
    }
}
