using FSNEP.BLL.Interfaces;
using FSNEP.Core.Abstractions;
using FSNEP.Core.Domain;
using UCDArch.Core.PersistanceSupport;

namespace FSNEP.BLL.Impl
{
    public class TimeRecordBLL : RecordBLL<TimeRecord>, ITimeRecordBLL
    {
        public TimeRecordBLL(IRepository repository, IMessageGateway messageGateway, ISignatureFactory signatureFactory) : base(repository, messageGateway, signatureFactory)
        {
        }
    }
}