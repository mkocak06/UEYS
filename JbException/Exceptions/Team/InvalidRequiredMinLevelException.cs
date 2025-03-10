using JbException.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace JbException.Exceptions.Team
{
    public class InvalidRequiredMinLevelException : JbBaseException
    {
        public InvalidRequiredMinLevelException(string message) : base(string.IsNullOrEmpty(message) ? "The required level for the team cannot be higher than the teamowner level." : message)
        {
        }

        public InvalidRequiredMinLevelException(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }

        public override string ErrorCode => "INVALID-REQ-MIN-LEVEl";
    }
}
