using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using FSNEP.BLL.Impl;
using FSNEP.BLL.Interfaces;
using FSNEP.Controllers;
using FSNEP.Core.Calendar;
using FSNEP.Core.Domain;
using FSNEP.Tests.Core.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Rhino.Mocks;
using UCDArch.Testing;
using UCDArch.Web.ActionResults;


namespace FSNEP.Tests.Controllers
{
    [TestClass]
    public class TimeRecordControllerTests : Core.ControllerTestBase<TimeRecordController>
    {
        private static readonly User User = CreateValidUser();
        private readonly ITimeRecordBLL _timeRecordBll = MockRepository.GenerateStub<ITimeRecordBLL>();
        private readonly TimeRecord _timeRecord = CreateValidTimeRecord();
        private readonly IUserBLL _userBll = MockRepository.GenerateStub<IUserBLL>();

        /// <summary>
        /// Override this method to setup a controller that doesn't have an empty ctor
        /// </summary>
        protected override void SetupController()
        {
            
            var timeRecordCalendarGenerator = new TimeRecordCalendarGenerator();
            _timeRecord.SetIdTo(1);
            _timeRecordBll.Expect(a => a.GetNullableByID(_timeRecord.Id)).Return(_timeRecord).Repeat.Any();
            _timeRecordBll.Expect(a => a.HasAccess("UserName", _timeRecord)).Return(true).Repeat.Any();
            _timeRecordBll.Expect(a => a.IsEditable(_timeRecord)).Return(true).Repeat.Any();

            CreateController(_timeRecordBll, _userBll, timeRecordCalendarGenerator);

            var fakeContext =
                MockRepository.GenerateStub<UCDArch.Core.PersistanceSupport.IDbContext>();
            _timeRecordBll.Expect(a => a.DbContext).Return(fakeContext).Repeat.Any();
        }

        #region Routing maps
        /// <summary>
        /// Routings the time record entry maps entry.
        /// </summary>
        [TestMethod]
        public void RoutingTimeRecordEntryMapsEntry()
        {
            const int id = 5;
            "~/TimeRecord/TimeRecordEntry/5"
                .ShouldMapTo<TimeRecordController>(a => a.TimeRecordEntry(id));
        }

        /// <summary>
        /// Routing add entry maps to add entry.
        /// </summary>
        [TestMethod]
        public void RoutingAddEntryMapsToAddEntry()
        {
            var timeRecordEntry = CreateValidTimeRecordEntry();
            //"~/TimeRecord/AddEntry?recordId=2".ShouldMapTo<TimeRecordController>(a => a.AddEntry(2, timeRecordEntry));
            //"~/TimeRecord/AddEntry".ShouldMapToIgnoringParams("TimeRecord", "AddEntry");
            "~/TimeRecord/AddEntry".Route().ShouldMapTo<TimeRecordController>(a => a.AddEntry(2, timeRecordEntry), true);
        }

        /// <summary>
        /// Routing remove entry maps to remove entry.
        /// </summary>
        [TestMethod]
        public void RoutingRemoveEntryMapsToRemoveEntry()
        {
//            "~/TimeRecord/RemoveEntry/2".ShouldMapToIgnoringParams("TimeRecord", "RemoveEntry");
            "~/TimeRecord/RemoveEntry".Route().ShouldMapTo<TimeRecordController>(a => a.RemoveEntry(2), true);
        }

        /// <summary>
        /// Routing edit entry maps to edit entry.
        /// </summary>
        [TestMethod]
        public void RoutingEditEntryMapsToEditEntry()
        {
            //"~/TimeRecord/EditEntry".ShouldMapToIgnoringParams("TimeRecord", "EditEntry");
            "~/TimeRecord/EditEntry".Route().ShouldMapTo<TimeRecordController>(a => a.EditEntry(1, null), true);
        }

        /// <summary>
        /// Routing get entry maps to get entry.
        /// </summary>
        [TestMethod]
        public void RoutingGetEntryMapsToGetEntry()
        {
            //"~/TimeRecord/GetEntry".ShouldMapToIgnoringParams("TimeRecord", "GetEntry");
            "~/TimeRecord/GetEntry".Route().ShouldMapTo<TimeRecordController>(a => a.GetEntry(1), true);
        }

        #endregion Routing maps

        #region AddEntry Tests

        /// <summary>
        /// Determines whether this instance [can add valid time record entry].
        /// </summary>
        [TestMethod]
        public void CanAddValidTimeRecordEntry()
        {
            var timeRecordEntry = CreateValidTimeRecordEntry();
            timeRecordEntry.SetIdTo(24);
            IPrincipal userPrincipal = new MockPrincipal();
            Controller.ControllerContext.HttpContext.User = userPrincipal;

            var timeRecord = _timeRecordBll.GetNullableByID(1);
            _timeRecordBll.AssertWasNotCalled(a => a.EnsurePersistent(timeRecord));

            var result = Controller.AddEntry(timeRecord.Id, timeRecordEntry);

            _timeRecordBll.AssertWasCalled(a => a.EnsurePersistent(timeRecord));
            Assert.IsNotNull(result);
            Assert.AreEqual("{ id = 24 }", result.Data.ToString());
        }

        /// <summary>
        /// Invalid time record id causes time record entry to fail.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UCDArch.Core.Utils.PreconditionException))]
        public void InvalidTimeRecordIdCausesTimeRecordEntryToFail()
        {
            const int invalidTimeRecordId = 123; //Does not exist and isn't mocked.
            JsonNetResult result = null;
            try
            {
                var timeRecordEntry = CreateValidTimeRecordEntry();
                timeRecordEntry.SetIdTo(24);
                IPrincipal userPrincipal = new MockPrincipal();
                Controller.ControllerContext.HttpContext.User = userPrincipal;                

                _timeRecordBll.AssertWasNotCalled(a => a.EnsurePersistent(_timeRecord));
                result = Controller.AddEntry(invalidTimeRecordId, timeRecordEntry);
            }
            catch (Exception message)
            {
                Assert.IsNotNull(_timeRecord);
                Assert.IsNull(result);
                Assert.AreEqual("Invalid time record indentifier", message.Message);
                _timeRecordBll.AssertWasNotCalled(a => a.EnsurePersistent(_timeRecord));
                throw;
            }
        }



        /// <summary>
        /// Time record with different username causes time record entry to fail.
        /// This test creates a timeRecord with an attached use with the usename "WrongOne".
        /// The mock that checks the currect user is hard coded to "UserName".
        /// That causes this to fail.
        /// Also, the GetNullableByID is set to use this timeRecord when it is called from within AddEntry. 
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UCDArch.Core.Utils.PreconditionException))]
        public void TimeRecordWithDifferentUsernameCausesTimeRecordEntryToFail()
        {
            TimeRecord timeRecord = null;
            JsonNetResult result = null;
            try
            {
                var timeRecordEntry = CreateValidTimeRecordEntry();
                timeRecordEntry.SetIdTo(24);
                IPrincipal userPrincipal = new MockPrincipal();
                Controller.ControllerContext.HttpContext.User = userPrincipal;
                timeRecord = CreateValidTimeRecord();
                timeRecord.User = CreateValidUser("WrongOne");
                timeRecord.SetIdTo(13);
                _timeRecordBll.Expect(a => a.GetNullableByID(timeRecord.Id)).Return(timeRecord).Repeat.Once();

                _timeRecordBll.AssertWasNotCalled(a => a.EnsurePersistent(timeRecord));
                result = Controller.AddEntry(timeRecord.Id, timeRecordEntry);
            }
            catch (Exception message)
            {
                Assert.IsNotNull(timeRecord);
                Assert.IsNull(result);
                Assert.AreEqual("Current user does not have access to this record", message.Message);
                _timeRecordBll.AssertWasNotCalled(a => a.EnsurePersistent(timeRecord));
                throw;
            }
        }

        /// <summary>
        /// Invalid time record entry causes time record entry to fail.
        /// Leonidas test (300th unit test created) :)
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UCDArch.Core.Utils.PreconditionException))]
        public void InvalidTimeRecordEntryCausesTimeRecordEntryToFail()
        {
            JsonNetResult result = null;
            try
            {
                var timeRecordEntry = CreateValidTimeRecordEntry();
                timeRecordEntry.ActivityType = null; //makes this invalid
                timeRecordEntry.SetIdTo(24);
                IPrincipal userPrincipal = new MockPrincipal();
                Controller.ControllerContext.HttpContext.User = userPrincipal;

                _timeRecordBll.AssertWasNotCalled(a => a.EnsurePersistent(_timeRecord));
                result = Controller.AddEntry(_timeRecord.Id, timeRecordEntry);
            }
            catch (Exception message)
            {
                Assert.IsNotNull(_timeRecord);
                Assert.IsNull(result);
                Assert.AreEqual("Entry is not valid", message.Message);
                _timeRecordBll.AssertWasNotCalled(a => a.EnsurePersistent(_timeRecord));
                throw;
            }
        }

        /// <summary>
        /// Invalid time record entry hours out of range causes time record entry to fail.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UCDArch.Core.Utils.PreconditionException))]
        public void InvalidTimeRecordEntryHoursOutOfRangeCausesTimeRecordEntryToFail()
        {
            JsonNetResult result = null;
            try
            {
                var timeRecordEntry = CreateValidTimeRecordEntry();
                timeRecordEntry.Hours = 25; //makes this invalid
                timeRecordEntry.SetIdTo(24);
                IPrincipal userPrincipal = new MockPrincipal();
                Controller.ControllerContext.HttpContext.User = userPrincipal;

                _timeRecordBll.AssertWasNotCalled(a => a.EnsurePersistent(_timeRecord));
                result = Controller.AddEntry(_timeRecord.Id, timeRecordEntry);
            }
            catch (Exception message)
            {
                Assert.IsNotNull(_timeRecord);
                Assert.IsNull(result);
                Assert.AreEqual("Entry is not valid", message.Message);
                _timeRecordBll.AssertWasNotCalled(a => a.EnsurePersistent(_timeRecord));
                throw;
            }
        } 

        #endregion AddEntry Tests

        #region TimeRecordEntry Tests
        /// <summary>
        /// Get time record entry returns correct view.
        /// </summary>
        [TestMethod]
        public void GetTimeRecordEntryReturnsCorrectView()
        {
            IPrincipal userPrincipal = new MockPrincipal();
            Controller.ControllerContext.HttpContext.User = userPrincipal;

            var projects = MockProjectsForUser();
            _userBll.Expect(a => a.GetUser()).IgnoreArguments().Return(User).Repeat.AtLeastOnce();
            var activityCategories = MockActivityCategories();

            var timeRecord = _timeRecordBll.GetNullableByID(1);
            var result = (ViewResult) Controller.TimeRecordEntry(timeRecord.Id);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.ViewData.ModelState.IsValid);
            result.ViewData.ModelState.AssertErrorsAre(); //No errors  

            var specialResult = (TimeRecordEntryViewModel)result.ViewData.Model;
            Assert.AreEqual(timeRecord, specialResult.TimeRecord);
            Assert.AreEqual(3, specialResult.ActivityCategories.Count);
            Assert.AreEqual(35, specialResult.CalendarDays.Count);
            Assert.AreEqual(1, specialResult.FundTypes.Count);
            Assert.AreEqual(2, specialResult.Projects.Count);
            Assert.IsFalse(specialResult.IsSubmittable, "If this one fails it is probably the test data.");
            foreach (var project in projects)
            {
                if (project.IsActive)
                {
                    Assert.IsTrue(specialResult.Projects.Contains(project));
                }
                else
                {
                    Assert.IsFalse(specialResult.Projects.Contains(project));
                }
            }
            foreach (var activityCategory in activityCategories)
            {
                if (activityCategory.IsActive)
                {
                    Assert.IsTrue(specialResult.ActivityCategories.Contains(activityCategory));
                }
                else
                {
                    Assert.IsFalse(specialResult.ActivityCategories.Contains(activityCategory));
                }
            }
        }

        /// <summary>
        /// Gets the time record entry redirects to home error when user has no access.
        /// </summary>
        [TestMethod]
        public void GetTimeRecordEntryRedirectsToHomeErrorWhenUserHasNoAccess()
        {
            IPrincipal userPrincipal = new MockPrincipal();
            Controller.ControllerContext.HttpContext.User = userPrincipal;

            MockProjectsForUser();
            _userBll.Expect(a => a.GetUser()).IgnoreArguments().Return(User).Repeat.AtLeastOnce();
            MockActivityCategories();

            //Specify a timeRecord with a different username that the "Current" faked one.
            var timeRecord = CreateValidTimeRecord();
            timeRecord.User = CreateValidUser("WrongOne");
            timeRecord.SetIdTo(13);
            _timeRecordBll.Expect(a => a.GetNullableByID(timeRecord.Id)).Return(timeRecord).Repeat.Once();

            Controller.TimeRecordEntry(timeRecord.Id)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Error(string.Format("{0} does not have access to this time record", timeRecord.User.UserName)));
        }

        /// <summary>
        /// Get time record entry with invalid time record throws exception.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UCDArch.Core.Utils.PreconditionException))]
        public void GetTimeRecordEntryWithInvalidTimeRecordThrowsException()
        {
            try
            {
                Controller.TimeRecordEntry(999);
            }
            catch (Exception message)
            {
                Assert.AreEqual("Invalid time record indentifier", message.Message);
                throw;
            }
            
        }

        #endregion TimeRecordEntry Tests

        #region GetEntry Tests
        /// <summary>
        /// Get entry returns json net result.
        /// </summary>
        [TestMethod]
        public void GetEntryReturnsJsonNetResult()
        {
            var timeRecordEntryRepository = FakeRepository<TimeRecordEntry>();
            Controller.Repository.Expect(a => a.OfType<TimeRecordEntry>()).Return(timeRecordEntryRepository).Repeat.
                Any();
            var timeRecordEntry = CreateValidTimeRecordEntry();
            timeRecordEntry.Project = new Project {IsActive = true, Name = "ProjectName1"};
            timeRecordEntry.FundType = new FundType { Name = "FundTypeName1" };
            timeRecordEntry.ActivityType = new ActivityType { IsActive = true, Name = "ActivityTypeName1" };
            timeRecordEntry.Account = new Account {IsActive = true, Name = "AccountName1"};
            timeRecordEntry.Comment = "CommentText";
            timeRecordEntry.SetIdTo(12);
            timeRecordEntryRepository.EnsurePersistent(timeRecordEntry);

            timeRecordEntryRepository.Expect(a => a.GetNullableByID(timeRecordEntry.Id)).Return(timeRecordEntry).Repeat.
                Once();

            var result = Controller.GetEntry(timeRecordEntry.Id);
            Assert.IsNotNull(result);
            Assert.AreEqual("{ Hours = 6.5, Id = 12, Comment = CommentText, ActivityType = ActivityTypeName1, Project = ProjectName1, Account = AccountName1, FundType = FundTypeName1 }", result.Data.ToString());

        }

        /// <summary>
        /// Gets the null time record entry by id redirects to error.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UCDArch.Core.Utils.PreconditionException))]
        public void GetNullTimeRecordEntryByIdRedirectsToError()
        {
            var timeRecordEntryRepository = FakeRepository<TimeRecordEntry>();
            Controller.Repository.Expect(a => a.OfType<TimeRecordEntry>()).Return(timeRecordEntryRepository).Repeat.
                Any();
            try
            {
                Controller.GetEntry(12);
            }
            catch (Exception message)
            {
                Assert.AreEqual("Entry not found", message.Message);
                throw;
            }
        }

        #endregion GetEntry Tests
        
        #region EditEntry Tests
        /// <summary>
        /// Edit entry copies comments and hours.
        /// </summary>
        [TestMethod]
        public void EditEntryCopiesCommentsAndHours()
        {
            var timeRecordEntryRepository = FakeRepository<TimeRecordEntry>();
            Controller.Repository.Expect(a => a.OfType<TimeRecordEntry>()).Return(timeRecordEntryRepository).Repeat.
                Any();

            var timeRecordEntry = CreateValidTimeRecordEntry();
            timeRecordEntry.Project = new Project { IsActive = true, Name = "ProjectName1" };
            timeRecordEntry.FundType = new FundType { Name = "FundTypeName1" };
            timeRecordEntry.ActivityType = new ActivityType { IsActive = true, Name = "ActivityTypeName1" };
            timeRecordEntry.Account = new Account { IsActive = true, Name = "AccountName1" };
            timeRecordEntry.Comment = "CommentText";
            timeRecordEntry.AdjustmentDate = DateTime.Now;
            timeRecordEntry.Record = new Record {ReviewComment = "test comment"};
            timeRecordEntry.SetIdTo(12);
            timeRecordEntryRepository.EnsurePersistent(timeRecordEntry);

            var timeRecordEntryToUpdate = CreateValidTimeRecordEntry();
            timeRecordEntryToUpdate.Comment = "This will be changed";
            timeRecordEntryToUpdate.Hours = 4;
            timeRecordEntryToUpdate.Date = 10;
            timeRecordEntryToUpdate.SetIdTo(14);

            timeRecordEntryRepository.Expect(a => a.GetNullableByID(timeRecordEntryToUpdate.Id)).Return(timeRecordEntryToUpdate).Repeat.
                Once();

            Controller.EditEntry(timeRecordEntryToUpdate.Id, timeRecordEntry);
            Assert.AreEqual(timeRecordEntry.Comment, timeRecordEntryToUpdate.Comment);
            Assert.AreEqual(timeRecordEntry.Hours, timeRecordEntryToUpdate.Hours);
            Assert.AreNotEqual(timeRecordEntry.Account, timeRecordEntryToUpdate.Account);
            Assert.AreNotEqual(timeRecordEntry.ActivityType, timeRecordEntryToUpdate.ActivityType);
            Assert.AreNotEqual(timeRecordEntry.AdjustmentDate, timeRecordEntryToUpdate.AdjustmentDate);
            Assert.AreNotEqual(timeRecordEntry.Date, timeRecordEntryToUpdate.Date);
            Assert.AreNotEqual(timeRecordEntry.FundType, timeRecordEntryToUpdate.FundType);
            Assert.AreNotEqual(timeRecordEntry.Id, timeRecordEntryToUpdate.Id);
            Assert.AreNotEqual(timeRecordEntry.Project, timeRecordEntryToUpdate.Project);
            Assert.AreNotEqual(timeRecordEntry.Record, timeRecordEntryToUpdate.Record);
        }
        #endregion

        #region Helper Methods


        /// <summary>
        /// Mocks the projects for user.
        /// </summary>
        /// <returns>List of Projects</returns>
        private List<Project> MockProjectsForUser()
        {
            var projects = new List<Project>
                               {
                                   new Project {Name = "Name", IsActive = true},
                                   new Project {Name = "NameInactive", IsActive = false},
                                   new Project {Name = "Name2", IsActive = true}
                               };
            projects[0].SetIdTo(2);
            projects[1].SetIdTo(3);
            projects[2].SetIdTo(4);
            _userBll.Expect(a => a.GetAllProjectsByUser(null)).IgnoreArguments().Return(projects.AsQueryable().Where(a =>a.IsActive)).Repeat.
                Once();
            return projects;
        }

        /// <summary>
        /// Mocks the activity categories.
        /// </summary>
        /// <returns>List of categories</returns>
        private List<ActivityCategory> MockActivityCategories()
        {
            var activityTypes = new List<ActivityType>
                                    {
                                        new ActivityType {Name = "ActivityType1", IsActive = true},
                                        new ActivityType {Name = "ActivityType2", IsActive = true}
                                    };

            var activityCategories = new List<ActivityCategory>
                                  {
                                      new ActivityCategory {Name = "ActivityCategory1", ActivityTypes = activityTypes, IsActive = true},
                                      new ActivityCategory {Name = "ActivityCategory2", ActivityTypes = activityTypes, IsActive = true},
                                      new ActivityCategory {Name = "ActivityCategoryInActive", ActivityTypes = activityTypes, IsActive = false},
                                      new ActivityCategory {Name = "ActivityCategory3", ActivityTypes = activityTypes, IsActive = true},
                                      
                                  };
            activityCategories[0].SetIdTo(1);
            activityCategories[1].SetIdTo(2);
            activityCategories[2].SetIdTo(3);
            activityCategories[3].SetIdTo(4);
            var activityCategoryRepository = FakeRepository<ActivityCategory>();
            Controller.Repository.Expect(a => a.OfType<ActivityCategory>()).Return(activityCategoryRepository).Repeat.
                Any();
            Controller.Repository.OfType<ActivityCategory>().Expect(a => a.Queryable).Return(activityCategories.AsQueryable()).Repeat.Any();
            return activityCategories;
        }


        /// <summary>
        /// Creates the valid time record.
        /// </summary>
        /// <returns></returns>
        private static TimeRecord CreateValidTimeRecord()
        {
            return new TimeRecord
                       {
                           Month = 10,
                           Year = 2009,
                           Salary = 200,
                           Status = new Status { Name = "S1" },
                           User = User,
                           ReviewComment = "A review is a review except when it isn't."
                       };
        }

        /// <summary>
        /// Creates the valid user.
        /// </summary>
        /// <returns></returns>
        private static User CreateValidUser()
        {
            var user = new User
            {
                FirstName = "FName",
                LastName = "LName",
                Salary = 1,
                BenefitRate = 2,
                FTE = 1,
                IsActive = true,
                UserName = "UserName"
            };
            user.Supervisor = user; //I'm my own supervisor
            user.Projects = new List<Project> {new Project {Name = "Project1"}};
            user.FundTypes = new List<FundType>{new FundType{Name = "FundType1"}};

            var userId = Guid.NewGuid();
            user.SetUserID(userId);

            return user;
        }

        /// <summary>
        /// Creates the valid user. With a specific UserName
        /// </summary>
        /// <param name="username">The username.</param>
        /// <returns></returns>
        private static User CreateValidUser(string username)
        {
            var user = CreateValidUser();
            user.UserName = username;
            return user;
        }

        /// <summary>
        /// Creates the valid time record entry.
        /// </summary>
        /// <returns></returns>
        private static TimeRecordEntry CreateValidTimeRecordEntry()
        {
            const int validDate = 25;
            const string validComment = "Comment";
            const double validHours = 6.5;

            return new TimeRecordEntry
                       {
                           Date = validDate,
                           Hours = validHours,
                           Comment = validComment,
                           Record = new Record(),
                           FundType = new FundType(),
                           Project = new Project(),
                           Account = new Account(),
                           ActivityType = new ActivityType()
                       };
        }

        #endregion Helper Methods

        #region mocks
        /// <summary>
        /// Mock the Identity. Used for getting the current user name
        /// </summary>
        public class MockIdentity : IIdentity
        {
            public string AuthenticationType
            {
                get
                {
                    return "MockAuthentication";
                }
            }

            public bool IsAuthenticated
            {
                get
                {
                    return true;
                }
            }

            public string Name
            {
                get
                {
                    return "UserName";
                }
            }
        }


        /// <summary>
        /// Mock the Principal. Used for getting the current user name
        /// </summary>
        public class MockPrincipal : IPrincipal
        {
            IIdentity _identity;

            public IIdentity Identity
            {
                get
                {
                    if (_identity == null)
                    {
                        _identity = new MockIdentity();
                    }
                    return _identity;
                }
            }

            public bool IsInRole(string role)
            {
                return false;
            }
        }

        /// <summary>
        /// Mock the HttpContext. Used for getting the current user name
        /// </summary>
        public class MockHttpContext : HttpContextBase
        {
            private IPrincipal _user;

            public override IPrincipal User
            {
                get
                {
                    if (_user == null)
                    {
                        _user = new MockPrincipal();
                    }
                    return _user;
                }
                set
                {
                    _user = value;
                }
            }
        }
        #endregion
    }
}
