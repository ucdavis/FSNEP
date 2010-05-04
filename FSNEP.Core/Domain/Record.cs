using System.Collections.Generic;
using UCDArch.Core.DomainModel;
using NHibernate.Validator.Constraints;
using System;
using UCDArch.Core.Utils;

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

        /// <summary>
        /// Readonly property that returns the full month name
        /// </summary>
        public virtual string MonthName
        {
            get
            {
                return Date.ToString("MMMM");
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

        /// <summary>
        /// Returns true if the record is in a state that it can be approved or denied.
        /// Really this is only if the record is pending review
        /// </summary>
        public virtual bool CanBeApprovedOrDenied
        {
            get
            {
                return Status.NameOption == Status.Option.PendingReview;
            }
        }

        /// <summary>
        /// Returns true if the sheet is in an editable state
        /// </summary>
        public virtual bool IsEditable
        {
            get
            {
                if (Status.NameOption == Status.Option.Current || Status.NameOption == Status.Option.Disapproved)
                {
                    return true; //editable only if the status is current or disapproved
                }

                return false;   
            }
        }

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