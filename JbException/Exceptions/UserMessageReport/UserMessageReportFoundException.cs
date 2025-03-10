using JbException.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace JbException.Exceptions.UserMessageReport
{
    public class UserMessageReportFoundException : JbBaseException
    {
        public UserMessageReportFoundException(long UserMessageReportId) : base($"The user message report Id {UserMessageReportId} not found.")
        {
        }

        public UserMessageReportFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }

        public override string ErrorCode => "USER-MESSAGE-REPORT-FOUND";
    }
}
