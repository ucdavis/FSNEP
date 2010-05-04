using System;
using System.Collections.Generic;
using FSNEP.Core.Domain;

namespace FSNEP.Core.Dto
{
    [Serializable]
    public class SignableCostShare : SignableRecord
    {
        public SignableCostShare(CostShare record, IEnumerable<CostShareEntry> entries) : base(record)
        {
            Entries = new List<SignableCostShareEntry>();

            foreach (var entry in entries)
            {
                var signableEntries = new SignableCostShareEntry(entry);
                Entries.Add(signableEntries);
            }
        }

        public ICollection<SignableCostShareEntry> Entries { get; private set; }
    }
}