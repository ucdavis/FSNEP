using System.Linq;
using CAESArch.BLL;

namespace FSNEP.BLL.Interfaces
{
    public interface ILookupBLL<T, IdT>
    {
        IQueryable<T> GetActive();
        INonStaticGenericBLLBase<T,IdT> Repository { get; set; }
    }
}