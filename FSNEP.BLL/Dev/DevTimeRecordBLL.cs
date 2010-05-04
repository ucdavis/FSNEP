using FSNEP.Core.Abstractions;
using FSNEP.Core.Domain;
using FSNEP.BLL.Interfaces;
using UCDArch.Core.PersistanceSupport;

namespace FSNEP.BLL.Dev
{
    public class DevTimeRecordBLL : DevRecordBLL<TimeRecord>, ITimeRecordBLL
    {
        public DevTimeRecordBLL(IRepository repository, IMessageGateway messageGateway) : base(repository, messageGateway)
        {
        }
    }
}