using System.Linq;
using CAESArch.BLL;
using FSNEP.Core.Domain;

namespace FSNEP.BLL.Impl
{
    public interface IActivityTypeBLL
    {
        IQueryable<ActivityCategory> GetActiveActivityCategories();
        IQueryable<ActivityType> GetActive();
        INonStaticGenericBLLBase<ActivityType, int> Repository { get; set; }
        INonStaticGenericBLLBase<ActivityCategory, int> GetActivityCategoryRepository();
    }

    public class ActivityTypeBLL : LookupBLL<ActivityType,int>, IActivityTypeBLL
    {
        public IQueryable<ActivityCategory> GetActiveActivityCategories()
        {
            return Repository.EntitySet<ActivityCategory>().Where(a => a.IsActive).OrderBy(a => a.Name);
        }

        public INonStaticGenericBLLBase<ActivityCategory, int> GetActivityCategoryRepository()
        {
            return new ActivityCategoryBLL().Repository;
        }

        private class ActivityCategoryBLL : LookupBLL<ActivityCategory,int>{}
    }
}