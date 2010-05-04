using System;
using FSNEP.BLL.Interfaces;
using FSNEP.Core.Domain;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;

namespace FSNEP.BLL.Dev
{
    public class RecordBLL<T> : IRecordBLL<T> where T : Record
    {
        private readonly IRepository _repository;

        public RecordBLL(IRepository repository)
        {
            _repository = repository;
        }

        public bool HasAccess(string userName, T record)
        {
            throw new NotImplementedException();
        }

        public bool IsEditable(T record)
        {
            Check.Require(record != null);

            if (record.Status.Name == "Current" || record.Status.Name == "Disapproved")
            {
                return true; //editable only if the status is current or disapproved
            }

            return false;
        }

        public bool IsSubmittable(T record)
        {
            throw new NotImplementedException();
        }

        public T GetCurrent(string userName)
        {
            throw new NotImplementedException();
        }
    }
}