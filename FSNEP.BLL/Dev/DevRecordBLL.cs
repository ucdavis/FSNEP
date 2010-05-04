using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using FSNEP.BLL.Interfaces;
using FSNEP.Core.Abstractions;
using FSNEP.Core.Domain;
using UCDArch.Core.PersistanceSupport;

namespace FSNEP.BLL.Dev
{
    public class DevRecordBLL<T> : IRecordBLL<T> where T : Record
    {
        private readonly IRepository _repository;
        private readonly IMessageGateway _messageGateway;

        public DevRecordBLL(IRepository repository, IMessageGateway messageGateway)
        {
            _repository = repository;
            _messageGateway = messageGateway;
        }

        public bool HasAccess(IPrincipal user, T record)
        {
            return true;
        }

        public bool HasReviewAccess(IPrincipal user, T record)
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
            _messageGateway.SendReviewMessage(record, approve);
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