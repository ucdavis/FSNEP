using FSNEP.BLL.Interfaces;
using NHibernate;
using CAESArch.Core.Utils;

namespace FSNEP
{
    public class AuditInterceptor : EmptyInterceptor
    {
        public IUserAuth UserAuth { get; private set; }

        public AuditInterceptor(IUserAuth userAuth)
        {
            Check.Require(userAuth != null, "User Authorization Context is Required");

            UserAuth = userAuth;
        }
        

    }
}