using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using FSNEP.BLL.Interfaces;
using FSNEP.Core.Domain;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;

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
            Check.Require(record != null);

            //if (record.Status.Name == Status.Option.Current.ToString() || record.Status.Name == Status.Option.Disapproved.ToString())
            if (record.Status.NameOption == Status.Option.Current || record.Status.NameOption == Status.Option.Disapproved)
            {
                return true; //editable only if the status is current or disapproved
            }

            return false;
        }

        public bool CanApproveOrDeny(T record)
        {
            return true;
        }

        public T GetCurrent(IPrincipal user)
        {
            return _repository.OfType<T>().Queryable.OrderBy(x=>x.Id).First();
        }

        public T GetCurrentRecord(IPrincipal user)
        {
            throw new NotImplementedException();
        }

        public DateTime GetCurrentSheetDate()
        {
            throw new NotImplementedException();
        }

        public void Submit(T record, IPrincipal user)
        {
            //Do nothing
        }

        public void ApproveOrDeny(T record, IPrincipal user, bool approve)
        {
            //Do nothing
        }

        /// <summary>
        /// Just return everything and order it correctly
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public IEnumerable<T> GetReviewableAndCurrentRecords(IPrincipal user)
        {
            var records = _repository.OfType<T>().Queryable
                .OrderBy(x => x.User.LastName)
                .ThenBy(x => x.Year)
                .ThenBy(x => x.Month);

            return records;
        }
    }
}