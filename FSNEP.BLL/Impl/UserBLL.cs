using System;
using System.Collections.Generic;
using System.Linq;
using FSNEP.BLL.Interfaces;
using FSNEP.Core.Domain;
using FSNEP.Core.Abstractions;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using UCDArch.Data.NHibernate;

namespace FSNEP.BLL.Impl
{
    public interface IUserBLL : IRepositoryWithTypedId<User,Guid>
    {
        IUserAuth UserAuth { get; set; }
        IQueryable<FundType> GetAvailableFundTypes(IRepository<FundType> fundTypeRepository);
        IQueryable<Project> GetAllProjectsByUser(IRepository<Project> projectRepository);
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

    public class UserBLL : RepositoryWithTypedId<User, Guid>, IUserBLL
    {
        public IUserAuth UserAuth { get; set; }

        public UserBLL(IUserAuth userAuth)
        {
            UserAuth = userAuth;
        }

        public IQueryable<FundType> GetAvailableFundTypes(IRepository<FundType> fundTypeRepository)
        {
            return fundTypeRepository.Queryable;
        }

        /// <summary>
        /// Gets all projects by user.
        /// All active projects will be returned if the current user is in the role "RoleNames.RoleAdmin"
        /// Otherwise, all active projects will be returned that are linked to the current user.
        /// </summary>
        /// <param name="projectRepository">The project repository.</param>
        /// <returns></returns>
        public IQueryable<Project> GetAllProjectsByUser(IRepository<Project> projectRepository)
        {            
            if (UserAuth.IsCurrentUserInRole(RoleNames.RoleAdmin))
            {
                return projectRepository.Queryable.Where(p => p.IsActive);
            }

            //if (UserAuth.IsCurrentUserInRole(RoleNames.RoleProjectAdmin))
            //{
            //    var currentUser = GetUser();                
            //    Check.Ensure(currentUser != null);
            //    Check.Ensure(currentUser.Projects != null, "User must have at least one project");
            //    return currentUser.Projects.Where(p => p.IsActive).AsQueryable();
            //}

            var currentUser = GetUser();
            Check.Ensure(currentUser != null);
            Check.Ensure(currentUser.Projects != null, "User must have at least one project");
            
            //Extra checks may not be needed
            var projects = currentUser.Projects.Where(p => p.IsActive).ToList();
            Check.Ensure(projects != null, "User must have at least one project");
            Check.Ensure(projects.Count() != 0, "User must have at least one active project");

            return currentUser.Projects.Where(p => p.IsActive).AsQueryable();
        }

        public IQueryable<User> GetSupervisors()
        {
            var userKeys = new HashSet<Guid>();

            foreach (var userName in UserAuth.RoleProvider.GetUsersInRole(RoleNames.RoleSupervisor))
            {
                userKeys.Add((Guid)UserAuth.GetUser(userName).ProviderUserKey);
            }

            var users = from usr in Queryable
                        where userKeys.ToList().Contains(usr.Id)
                        select usr;

            return users;
        }

        /// <summary>
        /// Gets the user from the UserAuth.CurrentUsername.
        /// </summary>
        /// <returns></returns>
        public User GetUser()
        {
            return GetUser(UserAuth.CurrentUserName);
        }

        public User GetUser(string username)
        {
            Check.Require(!string.IsNullOrEmpty(username), "Username can not be empty");

            var member = UserAuth.GetUser(username);

            return GetById((Guid)member.ProviderUserKey);
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
            return Queryable.Where(u => u.Supervisor.Id == user.Id);
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
                
                var currentUserProjectIds = GetUser().Projects.Select(p => p.Id).ToList(); //Get a list of projectIds for the current user
                
                var users = from u in Queryable
                            where u.IsActive && u.Projects.Any(p=>currentUserProjectIds.Contains(p.Id))
                            select u;

                return users;
            }

            return null;
        }
    }
}
