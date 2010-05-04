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
        /// Creates a valid Record Entity (Mostly)
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

        /// <summary>
        /// Creates a valid User Entity (Mostly)
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns></returns>
        public static User User(int? counter)
        {
            //TODO: Use this in the tests where "CreateValidUser" type calls are already being used.
            var extra = "";
            if (counter != null)
            {
                extra = counter.ToString();
            }

            var rtValue = new User();
            rtValue.FirstName = "FName" + extra;
            rtValue.LastName = "LName" + extra;
            rtValue.Salary = 10000.01;
            rtValue.BenefitRate = 2;
            rtValue.FTE = 1;
            rtValue.IsActive = true;
            rtValue.UserName = "UserName";
            rtValue.Supervisor = rtValue; //I'm my own supervisor
            rtValue.Projects = new List<Project> { new Project { Name = "Project1" } };
            rtValue.FundTypes = new List<FundType> { new FundType { Name = "FundType1" } };

            var userId = Guid.NewGuid();
            rtValue.SetUserID(userId);

            return rtValue;    
        }

        /// <summary>
        /// Creates a valid CostShare Entity (Mostly)
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns></returns>
        public static CostShare CostShare(int? counter)
        {
            var extra = "";
            if (counter != null)
            {
                extra = counter.ToString();
            }

            var rtValue = new CostShare
                              {
                                  Month = 10,
                                  Year = 2009,
                                  Status = new Status {NameOption = Status.Option.Current},
                                  User = new User(),
                                  ReviewComment = "ReviewComment" + extra
                              };

            return rtValue;
        }

        //TODO: add and use other entities
    }
}
