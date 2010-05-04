using System;
using System.IO;
using System.Linq;
using FSNEP.Core.Domain;
using FSNEP.Core.Dto;
using UCDArch.Core.PersistanceSupport;

namespace FSNEP.Core.Abstractions
{
    public interface ISignatureFactory
    {
        byte[] CreateSignature(Record record, IRepository repository);
    }

    public class SignatureFactory : ISignatureFactory
    {
        public byte[] CreateSignature(Record record, IRepository repository)
        {
            byte[] signature;

            if (record is TimeRecord)
            {
                var timeRecordEntries =
                    repository.OfType<TimeRecordEntry>().Queryable.Where(x => x.Record.Id == record.Id).OrderBy(x=>x.Id);

                var signableRecord = new SignableTimeRecord((TimeRecord) record, timeRecordEntries);

                signature = GenerateHash(signableRecord);
            }
            else if (record is CostShare)
            {
                var costShareEntries =
                    repository.OfType<CostShareEntry>().Queryable.Where(x => x.Record.Id == record.Id).OrderBy(x => x.Id);
                
                var signableRecord = new SignableCostShare((CostShare)record, costShareEntries);

                signature = GenerateHash(signableRecord);
            }
            else
            {
                throw new ArgumentException("Record must be a time or cost share record");
            }

            return signature;
        }

        protected byte[] GenerateHash(object signableRecord)
        {
            var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

            byte[] signature;

            using (var mstream = new MemoryStream())
            {
                formatter.Serialize(mstream, signableRecord);

                mstream.Seek(0, 0);

                signature = System.Security.Cryptography.SHA256.Create().ComputeHash(mstream);

                mstream.Close();
            }

            return signature;
        }
    }
}