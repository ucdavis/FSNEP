using System;
using CAESArch.Core.Domain;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

namespace FSNEP.Core.Domain
{
    public class SheetTracking : DomainObject<SheetTracking, int>
    {
        [IgnoreNulls]
        public virtual TimeSheet TimeSheet { get; set; }

        [IgnoreNulls]
        public virtual ExpenseSheet ExpenseSheet { get; set; }

        [NotNullValidator]
        public virtual Status Status { get; set; }

        [NotNullValidator]
        public virtual DateTime ActionDate { get; set; }

        [NotNullValidator]
        public virtual User User { get; set; }

        public SheetTracking()
        {

        }
    }
}