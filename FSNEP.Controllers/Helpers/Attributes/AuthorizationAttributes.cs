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

    public class AdminAndProjectAdmin : AuthorizeAttribute
    {
        public AdminAndProjectAdmin()
        {
            Roles = string.Format("{0}, {1}", RoleNames.RoleAdmin, RoleNames.RoleProjectAdmin);
        }
    }

    public class AllSupervisorsAttribute : AuthorizeAttribute
    {
        public AllSupervisorsAttribute()
        {
            Roles = string.Format("{0}, {1}", RoleNames.RoleSupervisor, RoleNames.RoleDelegateSupervisor);
        }
    }

    public class DirectSupervisorsOnlyAttribute : AuthorizeAttribute
    {
        public DirectSupervisorsOnlyAttribute()
        {
            Roles = RoleNames.RoleSupervisor;
        }
    }

    public class CostShareOnlyAttribute : AuthorizeAttribute
    {
        public CostShareOnlyAttribute()
        {
            Roles = RoleNames.RoleNonSalary;
        }
    }

    public class TimeRecordOnlyAttribute : AuthorizeAttribute
    {
        public TimeRecordOnlyAttribute()
        {
            Roles = RoleNames.RoleTimeSheet;
        }
    }
}