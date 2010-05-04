using FSNEP.Core.Domain;
using UCDArch.Core.PersistanceSupport;

namespace FSNEP.BLL.Interfaces
{
    public interface IRecordBLL<T> : IRepository<T> where T : Record
    {
        
    }
}