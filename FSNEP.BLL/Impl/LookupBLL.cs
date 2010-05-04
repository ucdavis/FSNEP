using CAESArch.BLL;
using CAESArch.Core.Domain;

namespace FSNEP.BLL.Impl
{
    public class LookupBLL<T, IdT> : LookupBLLBase<T, IdT> where T : LookupObject<T, IdT>, new()
    {

    }
}