using UCDArch.Core.NHibernateValidator.Extensions;
namespace FSNEP.Core.Domain
{
    /// <summary>
    /// Time Record inherits from record and provides an implementation suitable for timerecords
    /// </summary>
    public class TimeRecord : Record
    {
        [RangeDouble(Min = 0.01, Message = "Must be greater than zero")]
        public virtual double Salary { get; set; }

        public new virtual HoursInMonth HoursInMonth
        {
            get
            {
                return base.HoursInMonth;
            }
            set
            {
                base.HoursInMonth = value;
            }
        }

        public virtual double TargetHours
        {
            get
            {
                if (HoursInMonth == null) return 0;

                return User.FTE * HoursInMonth.Hours;
            }
        }
    }
}