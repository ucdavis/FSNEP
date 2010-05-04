using FSNEP.Core.Domain;

namespace FSNEP.BLL.Interfaces
{
    public interface IRecordBLL<T> where T : Record
    {
        /// <summary>
        /// Any user has access to any time record
        /// </summary>
        /// <returns></returns>
        bool HasAccess(string userName, T record);

        bool IsEditable(T record);

        /// <summary>
        /// Return the current time record if it has not been submitted.  If there is not current sheet, create one
        /// </summary>
        /// <param name="userName">Name of user who's time record should be found</param>
        /// <returns></returns>
        T GetCurrent(string userName);
    }
}