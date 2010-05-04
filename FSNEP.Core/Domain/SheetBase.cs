using System;
using System.Collections.Generic;
using CAESArch.Core.Domain;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

namespace FSNEP.Core.Domain
{
    public abstract class SheetBase<T, EntT, IdT> : DomainObject<T, IdT>
    {
        [RangeValidator(1, RangeBoundaryType.Inclusive, 12, RangeBoundaryType.Inclusive)] //Month is 1-12
            public virtual int Month { get; set; }

        /// <summary>
        /// Readonly property that initiates a date time with the current month and then returns the full month name
        /// </summary>
        public virtual string MonthName
        {
            get
            {
                return new DateTime(1, Month, 1).ToString("MMMM");
            }
        }

        [RangeValidator(2000, RangeBoundaryType.Inclusive, 9999, RangeBoundaryType.Inclusive)]
        public virtual int Year { get; set; }

        [NotNullValidator]
        public virtual User AssociatedUser { get; set; }

        [NotNullValidator]
        public virtual Status Status { get; set; }

        public virtual IList<EntT> Entries { get; set; }

        [IgnoreNulls]
        [StringLengthValidator(512)]
        public virtual string ReviewComment { get; set; }

        public virtual string UserName
        {
            get { return AssociatedUser.FullName; }
        }

        public SheetBase()
        {

        }
    }
}