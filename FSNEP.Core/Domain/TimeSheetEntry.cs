using System;
using CAESArch.Core.Domain;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

namespace FSNEP.Core.Domain
{
    public class TimeSheetEntry : DomainObject<TimeSheetEntry, int>, ISheetEntry, IComparable<TimeSheetEntry>
    {
        [NotNullValidator]
        public virtual TimeSheet AssociatedTimeSheet { get; set; }

        [RangeValidator(1, RangeBoundaryType.Inclusive, 31, RangeBoundaryType.Inclusive)] //Date is 1-31
            public virtual int Date { get; set; }

        [RangeValidator(0.00, RangeBoundaryType.Inclusive, 0.00, RangeBoundaryType.Ignore)]
        public virtual double Hours { get; set; }

        [NotNullValidator]
        public virtual Project Project { get; set; }

        [NotNullValidator]
        public virtual Account Account { get; set; }

        [NotNullValidator]
        public virtual FundType FundType { get; set; }

        [NotNullValidator]
        public virtual ActivityType ActivityType { get; set; }

        [IgnoreNulls]
        public virtual DateTime? AdjustmentDate { get; set; }

        [IgnoreNulls]
        [StringLengthValidator(256)]
        public virtual string Comment { get; set; }
        
        public TimeSheetEntry()
        {

        }

        #region ISheetEntry Members

        public virtual int GetID()
        {
            return this.ID;
        }

        #endregion

        #region IComparable<TimeSheetEntry> Members

        public virtual int CompareTo(TimeSheetEntry other)
        {
            return this.Date.CompareTo(other.Date);
        }

        #endregion
    }
}