using System.Security.Principal;
using System.Web.Security;
using FSNEP.BLL.Interfaces;
using FSNEP.Core.Abstractions;
using UCDArch.Core.Utils;

namespace FSNEP.BLL.Impl
{
    public class UserAuth : IUserAuth
    {
        public IPrincipal UserContext { get; set; }
        public IMembershipService MembershipService { get; set; }
        public RoleProvider RoleProvider { get; set; }

        public string CurrentUserName { get { return UserContext.Identity.Name; } }

        public UserAuth(IPrincipal userContext, IMembershipService membershipService, RoleProvider roleProvider)
        {
            UserContext = userContext;
            MembershipService = membershipService;
            RoleProvider = roleProvider;
        }

        public MembershipUser GetUser(string userName)
        {
            return MembershipService.GetUser(userName);
        }

        public MembershipUser GetUser()
        {
            Check.Require(UserContext.Identity.IsAuthenticated, "Can only get current user if user is authenticated");

            return MembershipService.GetUser(UserContext.Identity.Name);
        }

        public bool IsCurrentUserInRole(string roleName)
        {
            return UserContext.IsInRole(roleName);
        }
    }
}