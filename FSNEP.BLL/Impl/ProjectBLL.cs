using CAESArch.BLL;
using FSNEP.Core.Domain;

namespace FSNEP.BLL.Impl
{
    public interface IProjectBLL
    {
        INonStaticGenericBLLBase<Project, int> Repository { get; set; }
    }

    public class ProjectBLL : LookupBLL<Project,int>
    {
        
    }
}