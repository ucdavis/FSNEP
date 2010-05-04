using CAESArch.Core.Domain;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using System;

namespace FSNEP.Core.Domain
{
    public class HoursInMonth : DomainObject<HoursInMonth, YearMonthComposite>
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

        public override string ToString()
        {
            var date = new DateTime(ID.Year, ID.Month, 1);

            return string.Format("{0:MMMM yyyy}: {1} Hrs", date, Hours);
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

            var yearMonth = (YearMonthComposite)obj;

            //Equal if the year and month are exactly the same
            return yearMonth.Year == Year && yearMonth.Month == Month;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() + Year * 27 + Month * 7;
        }
    }
}