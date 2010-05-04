using FSNEP.BLL.Interfaces;
using FSNEP.Core.Domain;

namespace FSNEP.BLL.Impl
{
    public interface IAccountBLL : ILookupBLL<Account,int>
    {

    }

    public class AccountBLL : LookupBLL<Account,int>, IAccountBLL
    {
        
    }
}