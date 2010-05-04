using System;
using System.IO;
using FSNEP.Core.Domain;
using FSNEP.Core.Dto;

namespace FSNEP.Core.Abstractions
{
    public interface ISignatureFactory
    {
        byte[] CreateSignature(Record record);
    }

    public class SignatureFactory : ISignatureFactory
    {
        public byte[] CreateSignature(Record record)
        {
            byte[] signature;

            if (record is TimeRecord)
            {
                var signableRecord = new SignableTimeRecord((TimeRecord) record);

                signature = GenerateHash(signableRecord);
            }
            else if (record is CostShare)
            {
                var signableRecord = new SignableCostShare((CostShare) record);

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