using FSNEP.Core.Domain;

namespace FSNEP.BLL.Interfaces
{
    public interface ITimeRecordBLL : IRecordBLL<TimeRecord>
    {
        /// <summary>
        /// Any user has access to any time record
        /// </summary>
        /// <returns></returns>
        bool HasAccess(string userName, TimeRecord record);

        bool IsEditable(TimeRecord record);
        bool IsSubmittable(TimeRecord record);
    }
}