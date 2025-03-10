using JbException.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace JbException.Exceptions.UserTeam
{
    public class UserTeamCoFounderMaxMemberCount : JbBaseException
    {
        public UserTeamCoFounderMaxMemberCount(string teamName) : base($"The team name {teamName} Co founder member capacity is full.")
        {
        }

        public UserTeamCoFounderMaxMemberCount(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }

        public override string ErrorCode => "TEAM-COFOUNDER-MAX-MEMBER";
    }
}
