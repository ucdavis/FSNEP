using System.Linq;
using CAESArch.Core.Domain;

namespace FSNEP.BLL.Impl
{
    public class LookupBLL<T, IdT> : GenericBLL<T, IdT> where T : LookupObject<T, IdT>, new()
    {
        public IQueryable<T> GetActive()
        {
            return Repository.Queryable.Where(a => a.IsActive).OrderBy(a => a.Name);
        }
    }
}