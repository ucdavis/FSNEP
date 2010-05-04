using System.Collections.Generic;
using UCDArch.Core.NHibernateValidator.Extensions;

namespace FSNEP.Core.Domain
{
    public class Account : LookupObject
    {
        public virtual double IndirectCost { get; set; }

        [RangeDouble(0, 30)]
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

        public override string ToString()
        {
            return string.Format("{0} ({1}%)", Name, IndirectCostPercent);
        }
    }
}