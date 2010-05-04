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
    }
}