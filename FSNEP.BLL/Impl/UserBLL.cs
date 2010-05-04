using System;
using System.Linq;
using CAESArch.BLL;
using FSNEP.BLL.Interfaces;
using FSNEP.Core.Domain;

namespace FSNEP.BLL.Impl
{
    public interface IUserBLL
    {
        IUserAuth UserAuth { get; set; }
        INonStaticGenericBLLBase<User, Guid> Repository { get; set; }
        IQueryable<FundType> GetAvailableFundTypes();
        IQueryable<Project> GetAllActiveByUser();
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

        public IQueryable<Project> GetAllActiveByUser()
        {
            throw new NotImplementedException();
        }
    }
}
