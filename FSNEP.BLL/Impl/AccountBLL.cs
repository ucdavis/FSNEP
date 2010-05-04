using System.Linq;
using CAESArch.BLL;
using FSNEP.Core.Domain;

namespace FSNEP.BLL.Impl
{
    public interface IAccountBLL
    {
        IQueryable<Account> GetActive();
        INonStaticGenericBLLBase<Account, int> Repository { get; set; }
    }

    public class AccountBLL : LookupBLL<Account,int>, IAccountBLL
    {
        
    }
}