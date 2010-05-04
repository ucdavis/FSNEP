using System;
using System.Linq;
using System.Text;
using FSNEP.Core.Domain;
using FSNEP.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Data.NHibernate;
using UCDArch.Testing.Extensions;
using RepositoryTestBase = FSNEP.Tests.Core.RepositoryTestBase;

namespace FSNEP.Tests.Repositories
{
    [TestClass]
    public class CostShareRepositoryTests : AbstractRepositoryRecordTests<CostShare>
    {
        #region Init

        /// <summary>
        /// Gets the valid entity with Record as the base type.
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid costShare entity</returns>
        protected override CostShare GetValid(int? counter)
        {
            var costShare = CreateValidEntities.CostShare(counter);
            costShare.Status = Repository.OfType<Status>().Queryable.First();
            costShare.User = Repository.OfType<User>().Queryable.First();
            return costShare;
        }

        /// <summary>
        /// Loads the data.
        /// </summary>
        protected override void LoadData()
        {
            base.LoadData();

            Repository.OfType<CostShare>().DbContext.BeginTransaction();
            LoadRecords();
            Repository.OfType<CostShare>().DbContext.CommitTransaction();

        }

        #endregion Init

        //All tests for costShare currently done in base class 
        //because costShare doesn't have more fields than the record entity

    }
}
