using CAESArch.Core.Domain;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

namespace FSNEP.Core.Domain
{
    public class HoursInMonth : LookupObject<HoursInMonth, YearMonthComposite>
    {
        [RangeValidator(1, RangeBoundaryType.Inclusive, 0, RangeBoundaryType.Ignore)] //greater than 0
        public virtual int Hours{ get; set; }

        public HoursInMonth()
        {

        }

        public HoursInMonth(int year, int month)
        {
            id = new YearMonthComposite(month, year);
        }
    }

    public class YearMonthComposite
    {
        public virtual int Month { get; set; }
        public virtual int Year { get; set; }

        public YearMonthComposite(int month, int year)
        {
            Month = month;
            Year = year;
        }

        public YearMonthComposite()
        {

        }

        public override bool Equals(object obj)
        {
            if ( obj == null ) return base.Equals(obj);

            YearMonthComposite yearMonth = (YearMonthComposite)obj;

            //Equal if the year and month are exactly the same
            if (yearMonth.Year == this.Year && yearMonth.Month == this.Month)
                return true;
            else
                return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() + this.Year * 27 + this.Month * 7;
        }
    }
}