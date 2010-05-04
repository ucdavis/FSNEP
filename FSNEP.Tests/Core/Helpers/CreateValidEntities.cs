using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FSNEP.Core.Domain;

namespace FSNEP.Tests.Core.Helpers
{
    public static class CreateValidEntities
    {
        /// <summary>
        /// Create a valid entry for tests. 
        /// Repository tests may need to modify this data to supply real linked data.
        /// </summary>
        /// <returns></returns>
        public static Entry Entry(int? counter)
        {
            var extra = "";
            if (counter != null)
            {
                extra = counter.ToString();
            }
            return new Entry
                       {
                           Account = new Account(),
                           FundType = new FundType(),
                           Project = new Project(),
                           Record = new Record(),
                           Comment = "Comment" + extra
                       };
        }


        /// <summary>
        /// Records the specified counter.
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns></returns>
        public static Record Record(int? counter)
        {
            var extra = "";
            if (counter != null)
            {
                extra = counter.ToString();
            }

            return new Record
                       {
                           Month = 01, 
                           Year = 2009, 
                           Status = new Status(), 
                           User = new User(), 
                           ReviewComment = "ReviewComent" + extra
                       };
        }

        //TODO: add and use other entities
    }
}
