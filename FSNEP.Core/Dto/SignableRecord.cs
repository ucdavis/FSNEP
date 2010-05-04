using System;
using FSNEP.Core.Domain;

namespace FSNEP.Core.Dto
{
    [Serializable]
    public abstract class SignableRecord
    {
        protected SignableRecord(Record record)
        {
            Month = record.Month;
            Year = record.Year;
            UserName = record.User.UserName;
        }

        public string UserName { get; private set; }

        public int Year { get; private set; }

        public int Month { get; private set; }
    }
}