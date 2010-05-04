using CAESArch.BLL;
using FSNEP.Core.Domain;

namespace FSNEP.BLL.Impl
{
    public interface IAccountBLL
    {
        INonStaticGenericBLLBase<Account, int> Repository { get; set; }
    }

    public class AccountBLL : GenericBLL<Account,int>, IAccountBLL
    {
        
    }
}