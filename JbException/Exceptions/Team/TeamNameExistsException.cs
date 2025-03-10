using JbException.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace JbException.Exceptions.Team
{
    public class TeamNameExistsException : JbBaseException
    {
        public TeamNameExistsException(string teamName) : base($"The team name  {teamName} has been used before")
        {
        }

        public TeamNameExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }

        public override string ErrorCode => "TEAM-NAME-EXISTS";
    }
}
