using System;
using CAESArch.Core.Domain;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

namespace FSNEP.Core.Domain
{
    public class ExpenseSheetEntry : DomainObject<ExpenseSheetEntry, int>, ISheetEntry, IComparable<ExpenseSheetEntry>
    {
        [NotNullValidator]
        public virtual ExpenseSheet AssociatedExpenseSheet { get; set; }

        [NotNullValidator]
        public virtual FundType FundType { get; set; }

        [NotNullValidator]
        public virtual Project Project { get; set; }

        [NotNullValidator]
        public virtual Account Account { get; set; }

        [NotNullValidator]
        public virtual ExpenseType ExpenseType { get; set; }

        [RangeValidator(0.00, RangeBoundaryType.Exclusive, 0.00, RangeBoundaryType.Ignore)] // > 0
        public virtual double Amount { get; set; }

        [NotNullValidator]
        [StringLengthValidator(128)]
        public virtual string Description { get; set; }

        [IgnoreNulls]
        [StringLengthValidator(256)]
        public virtual string Comment { get; set; }

        public ExpenseSheetEntry()
        {

        }

        #region ISheetEntry Members

        public virtual int GetID()
        {
            return this.ID;
        }

        #endregion

        #region IComparable<ExpenseSheetEntry> Members

        public virtual int CompareTo(ExpenseSheetEntry other)
        {
            return this.Amount.CompareTo(other.Amount);
        }

        #endregion
    }
}
