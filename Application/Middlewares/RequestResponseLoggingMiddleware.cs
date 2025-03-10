using Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Shared.RequestModels;
using Shared.ResponseModels.Wrapper;
using System.Net;
using System.Text;

namespace Application.Middlewares
{
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger _logger;
        public RequestResponseLoggingMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            this.next = next;
            _logger = loggerFactory.CreateLogger<RequestResponseLoggingMiddleware>();
        }

        public async Task Invoke(HttpContext context, ILogService logService)
        {
            string requestBody = "";
            var param = new
            {
                UserIPAddress = context.Connection.RemoteIpAddress?.ToString(),
                UserId = Convert.ToInt64(context.User.Claims.FirstOrDefault()?.Value)
            };
            LogDTO log = new()
            {
                UserIPAddress = param.UserIPAddress,
                UserId = param.UserId,
                StartDate = DateTime.UtcNow,
                Message = $"Request.Method : {context.Request?.Method}"
                //Message = $"Request.Method : {context.Request?.Method}, Request.Path : {context.Request?.Path.Value}, " +
                //        $"Response.StatusCode : {context.Response?.StatusCode}",
            };
            try
            {
                context.Request.EnableBuffering();
                using StreamReader reader = new(context.Request.Body, Encoding.UTF8, true, 1024, true);
                requestBody = await reader.ReadToEndAsync();
            }
            finally
            {
                context.Request.Body.Position = 0;
            }
            try
            {
                await next(context);
                log.EndDate = DateTime.UtcNow;
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                await HandleExceptionAsync(context, ex);
            }
            finally
            {
                string message;

                if (context.Request?.Path != null && context.Request?.Path.Value != "/api/Authentication/Login")
                {
                    if (context.Request?.Path != null && context.Request.Path.Value.Contains("/api/Document/"))
                    {
                        message = $"Request.Method : {context.Request?.Method} , Request.Path : {context.Request?.Path.Value} , " +
                            $"QueryString : {context.Request?.QueryString.ToString()} , " +
                            $"Response.StatusCode : {context.Response?.StatusCode}";
                        log.Message = message;
                    }
                    else
                    {
                        message = $"Request.Method : {context.Request?.Method} , Request.Path : {context.Request?.Path.Value} , " +
                            $"QueryString : {context.Request?.QueryString.ToString()} , " +
                            "RequestBody : " + requestBody + " , " +
                            $"Response.StatusCode : {context.Response?.StatusCode}";
                        message = message.Replace("{", "{{").Replace("}", "}}");

                        log.Message = message;
                    }
                }
                else
                {
                    message = $"Request.Method : {context.Request?.Method} , Request.Path : {context.Request?.Path.Value} , " +
                           $"QueryString : {context.Request?.QueryString.ToString()} , " +
                           $"Response.StatusCode : {context.Response?.StatusCode}";
                    log.Message = message;
                }


                if (context.Response?.StatusCode is (int)HttpStatusCode.OK or (int)HttpStatusCode.Created)
                {
                    log.LogType = Shared.Types.LogType.Information;
                    _logger.LogInformation(message, param);
                }
                else if (context.Response?.StatusCode >= 500)
                {
                    log.LogType = Shared.Types.LogType.Error;
                    _logger.LogError(message, param);
                }
                else if (context.Response?.StatusCode is (int)HttpStatusCode.Forbidden or (int)HttpStatusCode.NotFound or (int)HttpStatusCode.BadRequest or (int)HttpStatusCode.Unauthorized)
                {
                    log.LogType = Shared.Types.LogType.Warning;
                    _logger.LogWarning(message, param);
                }
                else
                {
                    log.LogType = Shared.Types.LogType.Warning;
                    _logger.LogWarning(message, param);
                }
                await logService.PostAsync(default, log);
            }
        }
        //public static string GetUserIP(HttpContext context)
        //{
        //    var ip = (context.Connection.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != null
        //    && context.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != "")
        //    ? context.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"]
        //    : context.Current.Request.ServerVariables["REMOTE_ADDR"];
        //    if (ip.Contains(","))
        //        ip = ip.Split(',').First().Trim();
        //    return ip;
        //}
        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            
            await context.Response.WriteAsync(Newtonsoft.Json.JsonConvert.SerializeObject(new ResponseWrapper<Exception>()
            {
                Result = false,
                Message = "Bir Hata Oluştu! Lütfen sistem yöneticisi ile iletişime geçiniz ",
                Item = exception
            }));
        }
    }
}
