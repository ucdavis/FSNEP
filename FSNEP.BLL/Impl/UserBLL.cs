using System;
using FSNEP.Core.Domain;

namespace FSNEP.BLL.Impl
{
    public class UserBLL : GenericBLL<User,Guid>
    {
        public IUserAuth UserAuth { get; set; }

        public UserBLL(IUserAuth userAuth)
        {
            UserAuth = userAuth;
        }
    }
}
