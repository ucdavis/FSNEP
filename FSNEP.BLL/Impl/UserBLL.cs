using System;
using CAESArch.BLL;
using FSNEP.BLL.Interfaces;
using FSNEP.Core.Domain;

namespace FSNEP.BLL.Impl
{
    public interface IUserBLL
    {
        IUserAuth UserAuth { get; set; }
        INonStaticGenericBLLBase<User, Guid> Repository { get; set; }
    }

    public class UserBLL : GenericBLL<User,Guid>, IUserBLL
    {
        public IUserAuth UserAuth { get; set; }

        public UserBLL(IUserAuth userAuth)
        {
            UserAuth = userAuth;
        }
    }
}
