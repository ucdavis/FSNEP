using System;
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
        User GetUser();
        User GetUser(string username);
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
                Check.Ensure(currentUser.Projects != null);

                return currentUser.Projects.Where(p => p.IsActive).AsQueryable();
            }

            return null;
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
    }
}
