using NHibernate.Validator.Constraints;
using UCDArch.Core.NHibernateValidator.Extensions;

namespace FSNEP.Core.Domain
{
    /// <summary>
    /// Saves time record entries
    /// </summary>
    public class TimeRecordEntry : Entry
    {
        [Range(0,31)]
        public virtual int Date { get; set; }

        [RangeDouble(0,24)]
        public virtual double Hours { get; set; }
    }
}