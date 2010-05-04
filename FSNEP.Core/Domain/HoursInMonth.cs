using System;
using NHibernate.Validator.Constraints;
using NHibernate.Validator.Engine;
using UCDArch.Core.DomainModel;
using UCDArch.Core.NHibernateValidator.CommonValidatorAdapter;

namespace FSNEP.Core.Domain
{
    public class HoursInMonth : DomainObjectWithTypedId<YearMonthComposite>
    {
        [Min(1)]
        public virtual int Hours{ get; set; }

        public HoursInMonth()
        {

        }

        [NotNull (Message = "cannot be greater than 2 years from now or less than 1900")]
        public virtual string Year { get; set; }

        [NotNull(Message = "The year and month entered are not valid")]
        public override YearMonthComposite Id
        {
            get
            {
                return base.Id;
            }
            protected set
            {
                base.Id = value;
                try
                {
                    Year = Id.Year.ToString();
                    var date = new DateTime(Id.Year, Id.Month, 1);
                    if(Id.Year < 1900 || Id.Year > DateTime.Now.Year + 2)
                    {
                        Year = null;
                    }                    
                }
                catch (Exception)
                {
                    base.Id = null;
                }                                
            }
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