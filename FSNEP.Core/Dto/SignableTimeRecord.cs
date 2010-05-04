using System.Collections.Generic;
using FSNEP.Core.Domain;
using System;

namespace FSNEP.Core.Dto
{
    [Serializable]
    public class SignableTimeRecord : SignableRecord
    {
        public SignableTimeRecord(TimeRecord record, IEnumerable<TimeRecordEntry> entries) : base(record)
        {
            Salary = record.Salary;

            Entries = new List<SignableTimeRecordEntry>();

            foreach (var entry in entries)
            {
                var signableEntries = new SignableTimeRecordEntry(entry);
                Entries.Add(signableEntries);
            }
        }

        public ICollection<SignableTimeRecordEntry> Entries { get; private set; }

        public double Salary { get; private set; }
    }
}