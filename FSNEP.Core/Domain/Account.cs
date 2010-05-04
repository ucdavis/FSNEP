using System.Collections.Generic;
using CAESArch.Core.Domain;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

namespace FSNEP.Core.Domain
{
    public class Account : LookupObject<Account, int>
    {
        [RangeValidator(0.00, RangeBoundaryType.Inclusive, 0.30, RangeBoundaryType.Inclusive)]
        public virtual double IndirectCost { get; set; }

        public virtual double IndirectCostPercent
        {
            get
            {
                return IndirectCost * 100.0;
            }
            set
            {
                IndirectCost = value / 100.0;
            }
        }

        public virtual IList<Project> Projects { get; set; }

        public Account()
        {

        }
    }
}