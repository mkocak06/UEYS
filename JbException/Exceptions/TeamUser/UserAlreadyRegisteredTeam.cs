using JbException.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace JbException.Exceptions.TeamUser
{
    public class UserAlreadyRegisteredTeam : JbBaseException
    {
        public UserAlreadyRegisteredTeam(string teamName,string userId) : base($"{userId} user is already registered in {teamName} team")
        {
        }

        public UserAlreadyRegisteredTeam(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }

        public override string ErrorCode => "USER-AlREADY-TEAM";
    }
}
