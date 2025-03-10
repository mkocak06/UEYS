using JbException.Base;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace JbException.Middleware
{
    public class ExceptionHandlingActionFilter : IAsyncExceptionFilter
    {
        private readonly IHostEnvironment hostEnvironment;
        public ExceptionHandlingActionFilter(IHostEnvironment hostEnvironment)
        {
            this.hostEnvironment = hostEnvironment;
        }
        public Task OnExceptionAsync(ExceptionContext context)
        {
            var exception = context.Exception;
            var isDevelopment = hostEnvironment.EnvironmentName.Equals(Environments.Development, StringComparison.InvariantCultureIgnoreCase);
            var baseException = exception as JbBaseException;
            if (baseException!=null||isDevelopment)
            {
                var errorMessage= string.Empty;
                if (baseException != null)
                    errorMessage = baseException.Message;
                else
                    errorMessage = exception.Message;
                        
                context.Result = new BadRequestObjectResult(new { message = errorMessage })
                {
                    ContentTypes = { Application.Json }
                };

            }
            else
            {
                context.Result = new BadRequestObjectResult(new { message = "An error occurred during the operation" })
                {
                    ContentTypes = { Application.Json }
                };
            }
            return Task.CompletedTask;

        }


    }
}
