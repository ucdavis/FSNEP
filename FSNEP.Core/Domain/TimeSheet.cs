using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

namespace FSNEP.Core.Domain
{
    public class TimeSheet : SheetBase<TimeSheet, TimeSheetEntry, int>
    {
        [RangeValidator(0.00, RangeBoundaryType.Exclusive, 0.00, RangeBoundaryType.Ignore)]
        public virtual double Salary { get; set; }
    }
}