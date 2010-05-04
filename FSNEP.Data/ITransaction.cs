using System;

namespace FSNEP.Data
{
    public interface ITransaction : IDisposable
    {
        void Begin();
        bool IsActive { get; }
        void Commit();
        void Rollback();
    }
}
