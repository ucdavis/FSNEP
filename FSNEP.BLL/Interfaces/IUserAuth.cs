using System.Security.Principal;
using System.Web.Security;
using FSNEP.Core.Abstractions;

namespace FSNEP.BLL.Interfaces
{
    public interface IUserAuth
    {
        IPrincipal UserContext { get; set; }
        IMembershipService MembershipService { get; set; }
        string CurrentUserName { get; }
        RoleProvider RoleProvider { get; set; }
        MembershipUser GetUser(string userName);
        MembershipUser GetUser();
        bool IsCurrentUserInRole(string roleName);
    }
}