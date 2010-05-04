using System;
using FSNEP.BLL.Interfaces;
using FSNEP.Core.Domain;
using UCDArch.Data.NHibernate;

namespace FSNEP.BLL.Dev
{
    public class DevRecordBLL<T> : Repository<T>, IRecordBLL<T> where T : Record
    {
        public bool HasAccess(string userName, T record)
        {
            return true;
        }

        public bool IsEditable(T record)
        {
            return true;
        }

        public bool IsSubmittable(T record)
        {
            return true;
        }

        public T GetCurrent(string userName)
        {
            throw new NotImplementedException();
        }
    }
}