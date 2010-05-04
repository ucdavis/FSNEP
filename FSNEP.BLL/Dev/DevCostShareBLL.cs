using FSNEP.BLL.Interfaces;
using FSNEP.Core.Domain;
using UCDArch.Core.PersistanceSupport;

namespace FSNEP.BLL.Dev
{
    public class DevCostShareBLL : DevRecordBLL<CostShare>, ICostShareBLL
    {
        public DevCostShareBLL(IRepository repository) : base(repository)
        {
        }
    }
}