using System;
using System.Security.Principal;
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

        public bool HasAccess(IPrincipal user, T record)
        {
            return true;
        }

        public bool HasReviewAccess(IPrincipal user, T record)
        {
            return true;
        }

        public bool IsEditable(T record)
        {
            return true;
        }

        public T GetCurrent(IPrincipal user)
        {
            return _repository.OfType<T>().GetById(2);
        }

        public T GetCurrentRecord(IPrincipal user)
        {
            throw new NotImplementedException();
        }

        public DateTime GetCurrentSheetDate()
        {
            throw new NotImplementedException();
        }
    }
}