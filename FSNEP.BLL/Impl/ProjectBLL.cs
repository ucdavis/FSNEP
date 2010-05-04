using FSNEP.BLL.Interfaces;
using FSNEP.Core.Domain;

namespace FSNEP.BLL.Impl
{
    public interface IProjectBLL : ILookupBLL<Project,int>
    {
        
    }

    public class ProjectBLL : LookupBLL<Project,int>, IProjectBLL
    {

    }
}