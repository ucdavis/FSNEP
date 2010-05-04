using System;
using System.Web.Security;
using System.Security.Principal;
using System.Web;

namespace FSNEP.Core.Abstractions
{
    // The FormsAuthentication type is sealed and contains static members, so it is difficult to
    // unit test code that calls its members. The interface and helper class below demonstrate
    // how to create an abstract wrapper around such a type in order to make the AccountController
    // code unit testable.

    public interface IFormsAuthentication
    {
        void SignIn(string userName, bool createPersistentCookie);
        void SignOut();
    }

    public class FormsAuthenticationService : IFormsAuthentication
    {
        public void SignIn(string userName, bool createPersistentCookie)
        {
            FormsAuthentication.SetAuthCookie(userName, createPersistentCookie);
        }
        public void SignOut()
        {
            FormsAuthentication.SignOut();
        }
    }

    public class RoleProviderService : RoleProvider
    {
        public override bool IsUserInRole(string username, string roleName)
        {
            return Roles.IsUserInRole(username, roleName);
        }

        public override string[] GetRolesForUser(string username)
        {
            return Roles.GetRolesForUser(username);
        }

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            return Roles.RoleExists(roleName);
        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            Roles.AddUsersToRoles(usernames, roleNames);
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            Roles.RemoveUsersFromRoles(usernames, roleNames);
        }

        public override string[] GetUsersInRole(string roleName)
        {
            return Roles.GetUsersInRole(roleName);
        }

        public override string[] GetAllRoles()
        {
            return Roles.GetAllRoles();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string ApplicationName
        {
            get { return Roles.ApplicationName; }
            set { throw new NotImplementedException(); }
        }
    }

    public interface IMembershipService
    {
        int MinPasswordLength { get; }

        bool ValidateUser(string userName, string password);
        MembershipCreateStatus CreateUser(string userName, string password, string email);
        bool ChangePassword(string userName, string oldPassword, string newPassword);
        MembershipUser GetUser(string username);
        MembershipUser CreateUser(string userName, string password, string email, string question, string answer, bool isApproved, object providerUserKey, out MembershipCreateStatus status);
        void DeleteUser(string username);
        MembershipUser GetUser(object providerUserKey);
        bool ChangePasswordQuestionAndAnswer(string username, string password, string question, string answer);
    }

    public class WebPrincipal : IPrincipal
    {
        public bool IsInRole(string role)
        {
            return Roles.IsUserInRole(role);
        }

        public IIdentity Identity
        {
            get { return HttpContext.Current.User.Identity; }
        }
    }
    
    public class AccountMembershipService : IMembershipService
    {
        private MembershipProvider _provider;

        public AccountMembershipService()
            : this(null)
        {
        }

        public AccountMembershipService(MembershipProvider provider)
        {
            _provider = provider ?? Membership.Provider;
        }

        public int MinPasswordLength
        {
            get
            {
                return _provider.MinRequiredPasswordLength;
            }
        }

        public bool ValidateUser(string userName, string password)
        {
            return _provider.ValidateUser(userName, password);
        }

        public MembershipCreateStatus CreateUser(string userName, string password, string email)
        {
            MembershipCreateStatus status;
            _provider.CreateUser(userName, password, email, null, null, true, null, out status);
            return status;
        }

        public MembershipUser CreateUser(string userName, string password, string email, string question, string answer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {
            return _provider.CreateUser(userName, password, email, question, answer, isApproved, providerUserKey, out status);
        }

        public void DeleteUser(string username)
        {
            _provider.DeleteUser(username, true);
        }

        public bool ChangePassword(string userName, string oldPassword, string newPassword)
        {
            MembershipUser currentUser = _provider.GetUser(userName, true /* userIsOnline */);
            return currentUser.ChangePassword(oldPassword, newPassword);
        }

        public MembershipUser GetUser(string username)
        {
            return _provider.GetUser(username, true);
        }

        public MembershipUser GetUser(object providerUserKey)
        {
            return _provider.GetUser(providerUserKey, false);
        }

        public bool ChangePasswordQuestionAndAnswer(string username, string password, string question, string answer)
        {
            return _provider.ChangePasswordQuestionAndAnswer(username, password, question, answer);
        }
    }
}
