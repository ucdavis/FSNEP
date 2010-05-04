using System.Web.Mvc;
using FSNEP.Core.Abstractions;
using System;

namespace FSNEP.Controllers.Helpers.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AdminOnlyAttribute : AuthorizeAttribute
    {
        public AdminOnlyAttribute()
        {
            Roles = RoleNames.RoleAdmin;
        }
    }
}