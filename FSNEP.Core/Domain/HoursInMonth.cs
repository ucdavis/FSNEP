using System;
using NHibernate.Validator.Constraints;
using UCDArch.Core.DomainModel;

namespace FSNEP.Core.Domain
{
    public class HoursInMonth : DomainObjectWithTypedId<YearMonthComposite>
    {
        [Min(1)]
        public virtual int Hours{ get; set; }

        public HoursInMonth()
        {

        }

        public HoursInMonth(int year, int month)
        {
            Id = new YearMonthComposite(year, month);
        }

        /*
        //[SelfValidation]
        public virtual void Validate(ValidationResults results)
        {
            try
            {
                new DateTime(Id.Year, Id.Month, 1); //Try to create a new datetime using the id date values
            }
            catch (ArgumentOutOfRangeException)
            {
                results.AddResult(new ValidationResult("The year and month entered are not valid", this, "id", null,null));
            }
        }
         */

        public override string ToString()
        {
            var date = new DateTime(Id.Year, Id.Month, 1);

            return string.Format("{0:MMMM yyyy}: {1} Hrs", date, Hours);
        }
    }

    public class YearMonthComposite
    {
        public virtual int Month { get; set; }
        
        public virtual int Year { get; set; }

        public YearMonthComposite(int year, int month)
        {
            Month = month;
            Year = year;
        }

        public YearMonthComposite()
        {

        }

        public override bool Equals(object obj)
        {
            if (obj == null) return base.Equals(obj);

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