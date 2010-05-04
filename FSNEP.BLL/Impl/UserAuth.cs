using System.Security.Principal;
using System.Web.Security;
using FSNEP.Core.Abstractions;

namespace FSNEP.BLL.Impl
{
    public class UserAuth : IUserAuth
    {
        public IPrincipal UserContext { get; set; }
        public IMembershipService MembershipService { get; set; }

        public UserAuth(IPrincipal userContext, IMembershipService membershipService)
        {
            UserContext = userContext;
            MembershipService = membershipService;
        }

        public MembershipUser GetUser(string userName)
        {
            return MembershipService.GetUser(userName);
        }
    }
}