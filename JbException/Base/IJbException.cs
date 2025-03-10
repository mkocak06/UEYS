using JbException.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace JbException.Base
{
    public interface IJbException
    {
        string Message { get; }

        IDictionary Data { get; }

        Guid ExceptionId { get; }

        ExceptionLevel ExceptionLevel { get; }

        string ErrorCode { get; }

        HttpStatusCode HttpStatusCode { get; }
    }
}
