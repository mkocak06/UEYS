using JbException.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace JbException.Exceptions.UserMessageDetail
{
    public class MessageDoesNotBelongUserException : JbBaseException
    {
        public MessageDoesNotBelongUserException(string userId,long messageId) : base($"Message does not belong to user. UserName:{userId} MessageId:{messageId}")
        {
        }

        public MessageDoesNotBelongUserException(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }

        public override string ErrorCode => "USER-MESSAGE-NOT-BELONG-USER";
    }
}
