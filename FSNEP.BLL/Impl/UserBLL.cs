using System;
using System.Collections.Generic;
using System.Linq;
using CAESArch.BLL;
using CAESArch.Core.Utils;
using FSNEP.BLL.Interfaces;
using FSNEP.Core.Domain;
using FSNEP.Core.Abstractions;

namespace FSNEP.BLL.Impl
{
    public interface IUserBLL
    {
        IUserAuth UserAuth { get; set; }
        INonStaticGenericBLLBase<User, Guid> Repository { get; set; }
        IQueryable<FundType> GetAvailableFundTypes();
        IQueryable<Project> GetAllProjectsByUser();
        IQueryable<User> GetSupervisors();
        User GetUser();
        User GetUser(string username);
        List<string> GetAllRoles();
        IEnumerable<string> GetCurrentRoles();
        IEnumerable<string> GetUserRoles(string username);
    }

    public class UserBLL : GenericBLL<User,Guid>, IUserBLL
    {
        public IUserAuth UserAuth { get; set; }

        public UserBLL(IUserAuth userAuth)
        {
            UserAuth = userAuth;
        }

        public IQueryable<FundType> GetAvailableFundTypes()
        {
            return Repository.EntitySet<FundType>();
        }

        public IQueryable<Project> GetAllProjectsByUser()
        {
            if (UserAuth.IsCurrentUserInRole(RoleNames.RoleAdmin))
            {
                return Repository.EntitySet<Project>().Where(p => p.IsActive);
            }

            if (UserAuth.IsCurrentUserInRole(RoleNames.RoleProjectAdmin))
            {
                var currentUser = GetUser();
                
                Check.Ensure(currentUser != null);
                Check.Ensure(currentUser.Projects != null, "User must have at least one project");

                return currentUser.Projects.Where(p => p.IsActive).AsQueryable();
            }

            return null;
        }

        public IQueryable<User> GetSupervisors()
        {
            var userKeys = new HashSet<Guid>();

            foreach (var userName in UserAuth.RoleProvider.GetUsersInRole(RoleNames.RoleSupervisor))
            {
                userKeys.Add((Guid)UserAuth.GetUser(userName).ProviderUserKey);
            }

            var users = from usr in Repository.Queryable
                        where userKeys.ToList().Contains(usr.ID)
                        select usr;

            return users;
        }

        public User GetUser()
        {
            return GetUser(UserAuth.CurrentUserName);
        }

        public User GetUser(string username)
        {
            Check.Require(!string.IsNullOrEmpty(username), "Username can not be empty");

            var member = UserAuth.GetUser(username);

            return member == null ? null : Repository.GetNullableByID((Guid)member.ProviderUserKey);
        }

        public List<string> GetAllRoles()
        {
            var roleList = new List<string>(UserAuth.RoleProvider.GetAllRoles());

            if (!UserAuth.IsCurrentUserInRole(RoleNames.RoleAdmin)) //if user is not an admin, remove the visibility of that role
            {
                roleList.Remove(RoleNames.RoleAdmin);
            }

            return roleList;
        }

        public IEnumerable<string> GetCurrentRoles()
        {
            //Get roles for the current user
            return GetUserRoles(UserAuth.CurrentUserName);
        }

        public IEnumerable<string> GetUserRoles(string username)
        {
            return UserAuth.RoleProvider.GetRolesForUser(username);
        }
    }
}
