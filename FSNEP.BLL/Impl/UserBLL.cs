using System;
using System.Collections.Generic;
using System.Linq;
using CAESArch.BLL.Repositories;
using CAESArch.Core.DataInterfaces;
using CAESArch.Core.Utils;
using FSNEP.BLL.Interfaces;
using FSNEP.Core.Domain;
using FSNEP.Core.Abstractions;

namespace FSNEP.BLL.Impl
{
    public interface IUserBLL : IRepository<User>
    {
        IUserAuth UserAuth { get; set; }
        IQueryable<FundType> GetAvailableFundTypes();
        IQueryable<Project> GetAllProjectsByUser();
        IQueryable<User> GetSupervisors();
        User GetUser();
        User GetUser(string username);
        List<string> GetAllRoles();
        IEnumerable<string> GetCurrentRoles();
        IEnumerable<string> GetUserRoles(string username);
        void AddUserToRoles(string name, List<string> roleList);
        IQueryable<User> GetSubordinates(User user);
        void SetRoles(string username, List<string> roleList);
        IQueryable<User> GetAllUsers();
    }

    public class UserBLL : Repository<User>, IUserBLL
    {
        public IUserAuth UserAuth { get; set; }

        public UserBLL(IUserAuth userAuth)
        {
            UserAuth = userAuth;
        }

        public IQueryable<FundType> GetAvailableFundTypes()
        {
            return EntitySet<FundType>();
        }

        public IQueryable<Project> GetAllProjectsByUser()
        {
            if (UserAuth.IsCurrentUserInRole(RoleNames.RoleAdmin))
            {
                return EntitySet<Project>().Where(p => p.IsActive);
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

            var users = from usr in Queryable
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

            return GetByID((Guid)member.ProviderUserKey);
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

        public void AddUserToRoles(string name, List<string> roleList)
        {
            UserAuth.RoleProvider.AddUsersToRoles(new[] {name}, roleList.ToArray());
        }

        public IQueryable<User> GetSubordinates(User user)
        {
            //Get all users who have the current user as a supervisor
            return Queryable.Where(u => u.Supervisor.ID == user.ID);
        }

        public void SetRoles(string username, List<string> roleList)
        {
            Check.Require(roleList.Count > 0, "User must have at least one role");

            var existingRoles = GetUserRoles(username);

            //first remove the roles they currently have
            if (existingRoles.Count() > 0) UserAuth.RoleProvider.RemoveUsersFromRoles(new[] {username}, existingRoles.ToArray() );

            //now add in their new roles
            UserAuth.RoleProvider.AddUsersToRoles(new[] {username}, roleList.ToArray());
        }

        public IQueryable<User> GetAllUsers()
        {
            if (UserAuth.IsCurrentUserInRole(RoleNames.RoleAdmin))
            {
                //Admins can get all active users
                return Queryable.Where(u => u.IsActive).OrderBy(u => u.LastName);
            }
            
            if (UserAuth.IsCurrentUserInRole(RoleNames.RoleProjectAdmin))
            {
                //Filtered admin can only get users who are associated with their projects
                
                var currentUserProjectIds = GetUser().Projects.Select(p => p.ID).ToList(); //Get a list of projectIds for the current user
                
                var users = from u in Queryable
                            where u.IsActive && u.Projects.Any(p=>currentUserProjectIds.Contains(p.ID))
                            select u;

                return users;
            }

            return null;
        }
    }
}
