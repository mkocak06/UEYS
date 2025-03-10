
using JbException.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace JbException.Base
{
    [Serializable]
    public abstract class JbBaseException : Exception, IJbException
    {
        public const string DefaultErrorCode = "ERROR";

        protected readonly Dictionary<string, object> ExceptionData;

        protected JbBaseException(string message) :
            this(message, ExceptionLevel.Domain, DefaultErrorCode, null)
        {
        }

        protected JbBaseException(string message, Exception innerException) :
            this(message, ExceptionLevel.Domain, DefaultErrorCode, innerException)
        {
        }

        protected JbBaseException(string message, ExceptionLevel exceptionLevel) :
            this(message, exceptionLevel, DefaultErrorCode, null)
        {
        }

        protected JbBaseException(string message, string errorCode) :
            this(message, ExceptionLevel.Domain, errorCode, null)
        {
        }

        protected JbBaseException(string message, string errorCode, Exception innerException) :
            this(message, ExceptionLevel.Domain, errorCode, innerException)
        {
        }

        protected JbBaseException(string message, ExceptionLevel exceptionLevel, string errorCode) :
            this(message, exceptionLevel, errorCode, null)
        {
        }

        protected JbBaseException(string message, ExceptionLevel exceptionLevel, string errorCode, Exception innerException) : base(message, innerException)
        {
            ExceptionId = Guid.NewGuid();
            ErrorCode = errorCode ?? DefaultErrorCode;
            ExceptionLevel = exceptionLevel;
            ExceptionData = new Dictionary<string, object>();
        }

        protected JbBaseException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            ExceptionId = Guid.NewGuid();
            ExceptionData = new Dictionary<string, object>();
        }

        public override IDictionary Data
        {
            get
            {
                SetPropsToExceptionData();
                return ExceptionData;
            }
        }
        public Guid ExceptionId { get; }
        public virtual ExceptionLevel ExceptionLevel { get; }
        public virtual bool SkipMonitoring => false;
        public virtual bool SkipLogging => false;
        public virtual bool UseMessageForApiResult => false;
        public virtual string ErrorCode { get; }
        public virtual HttpStatusCode HttpStatusCode => ExceptionLevel == ExceptionLevel.Validation ? HttpStatusCode.BadRequest : HttpStatusCode.InternalServerError;

       

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null) throw new ArgumentNullException(nameof(info));
            SetPropsToSerializationInfo(info);
            base.GetObjectData(info, context);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(base.ToString());
            sb.AppendLine("");
            sb.AppendLine("     <---- EXCEPTION DATA ---->");
            sb.AppendLine($"      {JsonSerializer.Serialize(Data)}");
            sb.AppendLine("     <------------------------>");
            sb.AppendLine("");
            return sb.ToString();
        }

        private void SetPropsToExceptionData()
        {
            if (!ExceptionData.ContainsKey(nameof(ExceptionLevel))) ExceptionData.Add(nameof(ExceptionLevel), ExceptionLevel);
            if (!ExceptionData.ContainsKey(nameof(ErrorCode))) ExceptionData.Add(nameof(ErrorCode), ErrorCode);
            if (!ExceptionData.ContainsKey(nameof(HttpStatusCode))) ExceptionData.Add(nameof(HttpStatusCode), ((int)HttpStatusCode).ToString());
            if (!ExceptionData.ContainsKey("HttpStatusCodeName")) ExceptionData.Add("HttpStatusCodeName", ((int)HttpStatusCode).ToString());
        }

        private void SetPropsToSerializationInfo(SerializationInfo info)
        {
            info.AddValue(nameof(ErrorCode), ErrorCode);
            info.AddValue(nameof(ExceptionLevel), Enum.GetName(typeof(ExceptionLevel), ExceptionLevel));
            info.AddValue(nameof(HttpStatusCode), (int)HttpStatusCode);
            info.AddValue("HttpStatusCodeName", Enum.GetName(typeof(HttpStatusCode), HttpStatusCode));
        }
    }
}
