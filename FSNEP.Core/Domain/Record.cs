using System.Collections.Generic;
using UCDArch.Core.DomainModel;
using NHibernate.Validator.Constraints;
using System;

namespace FSNEP.Core.Domain
{
    public class Record : DomainObject
    {
        [Range(1, 12)]
        public virtual int Month { get; set; }

        [Min(1)]
        public virtual int Year { get; set; }

        public virtual DateTime Date
        {
            get
            {
                return new DateTime(Year, Month, 1);
            }
        }

        [NotNull]
        public virtual User User { get; set; }

        [NotNull]
        public virtual Status Status { get; set; }

        [Length(512)]
        public virtual string ReviewComment { get; set; }

        [NotNull] //Can be empty
        public virtual IList<Entry> Entries { get; set; }

        public Record()
        {
            Entries = new List<Entry>();
        }

        public virtual void AddEntry(Entry entry)
        {
            entry.Record = this;
            Entries.Add(entry);
        }
    }
}