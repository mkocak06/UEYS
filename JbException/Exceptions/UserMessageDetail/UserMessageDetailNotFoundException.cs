using JbException.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace JbException.Exceptions.UserMessageDetail
{
    public class UserMessageDetailNotFoundException : JbBaseException
    {
        public UserMessageDetailNotFoundException(string teamId) : base($"The user message detail Id {teamId} not found.")
        {
        }

        public UserMessageDetailNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }

        public override string ErrorCode => "USER-MESSAGE-DETAIL-NOT-FOUND";
    }
}
