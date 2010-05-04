using System;
using System.Collections.Generic;
using FSNEP.Core.Domain;

namespace FSNEP.Core.Dto
{
    [Serializable]
    public class SignableCostShare : SignableRecord
    {
        public SignableCostShare(CostShare record) : base(record)
        {
            Entries = new List<SignableCostShareEntry>();

            foreach (var entry in record.Entries)
            {
                var signableEntries = new SignableCostShareEntry((CostShareEntry) entry);
                Entries.Add(signableEntries);
            }
        }

        public ICollection<SignableCostShareEntry> Entries { get; private set; }
    }
}