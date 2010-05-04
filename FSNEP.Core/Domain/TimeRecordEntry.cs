using System;
using Newtonsoft.Json;
using NHibernate.Validator.Constraints;
using UCDArch.Core.NHibernateValidator.Extensions;

namespace FSNEP.Core.Domain
{
    /// <summary>
    /// Saves time record entries
    /// </summary>
    public class TimeRecordEntry : Entry
    {
        [Range(1,31)]
        [JsonProperty]
        public virtual int Date { get; set; }

        [RangeDouble(0,24)]
        [JsonProperty]
        public virtual double Hours { get; set; }

        [NotNull]
        public virtual ActivityType ActivityType { get; set; }

        public virtual DateTime? AdjustmentDate { get; set; }
    }
}