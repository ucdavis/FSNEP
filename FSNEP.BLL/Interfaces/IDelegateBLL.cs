using FSNEP.Core.Domain;

namespace FSNEP.BLL.Interfaces
{
    public interface IDelegateBLL
    {
        /// <summary>
        /// Assign the given user as a delegate for the current user
        /// </summary>
        void AssignDelegate(User userToAssign);

        void RemoveDelegate(User userToRemove);
    }
}