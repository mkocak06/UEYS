using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Memory;
using Shared.Extensions;
using Shared.Models;
using Shared.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Koru
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = false)]
    public class HasPermissionAttribute : AuthorizeAttribute
    {
        public HasPermissionAttribute(PermissionEnum permission) : base(permission.ToString())
        {
            var info = permission.GetAttribute<PermissionInformationAttribute>();
            KoruRepository.Add(permission.ToString(), info.Description, info.Group);
        }
    }
}
