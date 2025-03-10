using JbException.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace JbException.Exceptions.Team
{
    public class TeamNotFoundException : JbBaseException
    {
        public TeamNotFoundException(string teamId) : base($"The team Id {teamId} not found.")
        {
        }

        public TeamNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }

        public override string ErrorCode => "TEAM-NOT-FOUND";
    }
}
