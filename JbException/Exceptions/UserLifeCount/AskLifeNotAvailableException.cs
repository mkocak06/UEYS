using JbException.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace JbException.Exceptions.UserLifeCount
{
    public  class AskLifeNotAvailableException : JbBaseException
    {
        public AskLifeNotAvailableException(string userId) : base($"The user {userId} ask life 	not available")
        {
        }

        public AskLifeNotAvailableException(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }

        public override string ErrorCode => "ASK-LIFE-NOT-AVAILABLE";
    }
}
