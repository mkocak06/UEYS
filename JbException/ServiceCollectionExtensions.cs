using JbException.Middleware;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JbException
{
    public static class ServiceCollectionExtensions
    {
        public static MvcOptions AddApplicationLogging(this MvcOptions mvcOptions)
        {
            mvcOptions.Filters.Add<ExceptionHandlingActionFilter>();
            return mvcOptions;
        }
    }
}
