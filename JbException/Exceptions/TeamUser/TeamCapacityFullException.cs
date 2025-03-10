using JbException.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace JbException.Exceptions.TeamUser
{
    public  class TeamCapacityFullException : JbBaseException
    {
        public TeamCapacityFullException(string teamName) : base($"The team name {teamName} capacity is full.")
        {
        }

        public TeamCapacityFullException(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }

        public override string ErrorCode => "TEAM-CAPACITY-FULL";
    }
}
