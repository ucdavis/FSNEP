using System;
using FSNEP.BLL.Interfaces;
using FSNEP.Core.Domain;
using UCDArch.Core.PersistanceSupport;

namespace FSNEP.BLL.Dev
{
    public class DevRecordBLL<T> : IRecordBLL<T> where T : Record
    {
        protected readonly IRepository _repository;

        public DevRecordBLL(IRepository repository)
        {
            _repository = repository;
        }

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