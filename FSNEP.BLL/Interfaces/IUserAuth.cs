using System.Security.Principal;
using System.Web.Security;
using FSNEP.Core.Abstractions;

namespace FSNEP.BLL.Interfaces
{
    public interface IUserAuth
    {
        IPrincipal UserContext { get; set; }
        IMembershipService MembershipService { get; set; }
        MembershipUser GetUser(string userName);
    }
}