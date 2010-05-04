using System;
using CAESArch.BLL;
using CAESArch.BLL.Repositories;
using CAESArch.Core.DataInterfaces;
using FSNEP.Core.Domain;
using FSNEP.Tests.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FSNEP.Tests.Repositories
{
    [TestClass]
    public class HoursInMonthRepositoryTests : RepositoryTestBase
    {
        private readonly IRepository<HoursInMonth> _hoursInMonthRepository = new Repository<HoursInMonth>();

        /// <summary>
        /// Valid Hours
        /// </summary>
        public const int ValidValueHours = 100;
        /// <summary>
        /// Invalid Hours
        /// </summary>
        public const int InvalidValueHours = 0;

        //Don't load any data
        protected override void LoadData() { }

        [TestMethod]
        public void CanSaveCompleteAndValidHoursInMonth()
        {
            var hoursInMonth = new HoursInMonth(2009, 09) {Hours = ValidValueHours};

            using (var ts = new TransactionScope())
            {
                _hoursInMonthRepository.EnsurePersistent(hoursInMonth);

                ts.CommitTransaction();
            }

            Assert.AreEqual(false, hoursInMonth.IsTransient());
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void HoursInMonthDoesNotSaveWithZeroHours()
        {
            var hoursInMonth = new HoursInMonth(2009, 09) { Hours = InvalidValueHours };

            try
            {
                using (var ts = new TransactionScope())
                {
                    _hoursInMonthRepository.EnsurePersistent(hoursInMonth);

                    ts.CommitTransaction();
                }
            }
            catch(Exception message)
            {
                Assert.AreEqual("Object of type FSNEP.Core.Domain.HoursInMonth could not be persisted\n\n\r\nValidation Errors: Hours, The value must fall within the range \"1\" (Inclusive) - \"0\" (Ignore).\r\n", message.Message, "Expected Exception Not encountered");
                throw;
            }
        }
    }
}
