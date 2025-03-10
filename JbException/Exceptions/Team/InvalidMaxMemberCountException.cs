using JbException.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace JbException.Exceptions.Team
{
    public class InvalidMaxMemberCountException : JbBaseException
    {
        public InvalidMaxMemberCountException( string message) : base(string.IsNullOrEmpty(message) ? "MaxMemberCount value must be greater than 1" : message)
        {
        }

        public InvalidMaxMemberCountException(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }

        public override string ErrorCode => "INVALID-MAX-MEMBER-COUNT";
    }
}
