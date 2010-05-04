using FSNEP.BLL.Interfaces;
using FSNEP.Core.Domain;
using UCDArch.Data.NHibernate;

namespace FSNEP.BLL.Dev
{
    public class DevTimeRecordBLL : Repository<TimeRecord>, ITimeRecordBLL
    {
        /// <summary>
        /// Any user has access to any time record
        /// </summary>
        /// <returns></returns>
        public bool HasAccess(string userName, TimeRecord record)
        {
            return true;
        }

        public bool IsEditable(TimeRecord record)
        {
            return true;
        }

        public bool IsSubmittable(TimeRecord record)
        {
            return true;
        }
    }
}