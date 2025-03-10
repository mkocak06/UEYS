using JbException.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace JbException.Exceptions.SpecDetail
{
    public class SpecDetailCodeExistsException : JbBaseException
    {
        public SpecDetailCodeExistsException(string specDetailCode) : base($"The spec detail code {specDetailCode} already exists.")
        {
        }

        public SpecDetailCodeExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }

        public override string ErrorCode => "SPEC-DETAIL-CODE-EXISTS";
    }
}
