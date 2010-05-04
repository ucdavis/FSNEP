using System.Security.Principal;
using System.Web.Security;
using FSNEP.BLL.Interfaces;
using FSNEP.Core.Abstractions;
using CAESArch.Core.Utils;

namespace FSNEP.BLL.Impl
{
    public class UserAuth : IUserAuth
    {
        public IPrincipal UserContext { get; set; }
        public IMembershipService MembershipService { get; set; }

        public string CurrentUserName { get { return UserContext.Identity.Name; } }

        public UserAuth(IPrincipal userContext, IMembershipService membershipService)
        {
            UserContext = userContext;
            MembershipService = membershipService;
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