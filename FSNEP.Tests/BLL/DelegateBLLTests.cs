using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;
using FSNEP.BLL.Dev;
using FSNEP.BLL.Impl;
using FSNEP.BLL.Interfaces;
using FSNEP.Core.Abstractions;
using FSNEP.Core.Domain;
using FSNEP.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace FSNEP.Tests.BLL
{
    [TestClass]
    public class DelegateBLLTests
    {
        private DelegateBLL _delegateBLL;
        private IUserAuth _userAuth;
        private IUserBLL _userBLL;
        private RoleProvider _roleProvider;
        private readonly User CurrentUser = CreateValidEntities.User(1);
        private List<User> _users = new List<User>();

        [TestInitialize]
        public void Setup()
        {
            _userBLL = MockRepository.GenerateStub<IUserBLL>();
            _userAuth = MockRepository.GenerateStub<IUserAuth>();
            _delegateBLL = new DelegateBLL(_userAuth, _userBLL);
            _roleProvider = MockRepository.GenerateStub<RoleProvider>();
            _userAuth.RoleProvider = _roleProvider;

            CurrentUser.UserName = "CurrentUser";
            _userBLL.Expect(a => a.GetUser()).Return(CurrentUser).Repeat.Any();

            for (int i = 0; i < 3; i++)
            {
                _users.Add(CreateValidEntities.User(i+3));
            }
        }

        #region AssignDelegate Tests

        /// <summary>
        /// Tests the assign delegate is saved when current user is in supervisor role.
        /// </summary>
        [TestMethod]
        public void TestAssignDelegateIsSavedWhenCurrentUserIsInSupervisorRole()
        {
            const string userToAssignUserName = "UserToAssign";
            _userAuth.Expect(a => a.IsCurrentUserInRole(RoleNames.RoleSupervisor)).Return(true).Repeat.Once();
            _userAuth.RoleProvider
                .Expect(a => a.IsUserInRole(userToAssignUserName, RoleNames.RoleDelegateSupervisor))
                .Return(true).Repeat.Once();

            var userToAssign = CreateValidEntities.User(2);
            userToAssign.UserName = userToAssignUserName;

            CurrentUser.Delegate = null;

            _delegateBLL.AssignDelegate(userToAssign);
            _userBLL.AssertWasCalled(a => a.EnsurePersistent(CurrentUser));

            Assert.AreSame(userToAssign, CurrentUser.Delegate);
        }

        /// <summary>
        /// Tests the assign delegate calls add _users to roles when user does not have role delegate supervisor.
        /// </summary>
        [TestMethod]
        public void TestAssignDelegateCallsAddUsersToRolesWhenUserDoesNotHaveRoleDelegateSupervisor()
        {
            const string userToAssignUserName = "UserToAssign";
            _userAuth.Expect(a => a.IsCurrentUserInRole(RoleNames.RoleSupervisor)).Return(true).Repeat.Once();
            _userAuth.RoleProvider
                .Expect(a => a.IsUserInRole(userToAssignUserName, RoleNames.RoleDelegateSupervisor))
                .Return(false).Repeat.Once();
           

            var userToAssign = CreateValidEntities.User(2);
            userToAssign.UserName = userToAssignUserName;

            CurrentUser.Delegate = null;

            _delegateBLL.AssignDelegate(userToAssign);
            _userBLL.AssertWasCalled(a => a.EnsurePersistent(CurrentUser));

            var argumentsForCallsMadeOn = _roleProvider.GetArgumentsForCallsMadeOn(a => a.AddUsersToRoles(Arg<string[]>.Is.Anything, Arg<string[]>.Is.Anything));
            Assert.AreEqual(1, argumentsForCallsMadeOn.Count(), "AddUsersToRoles should have been called once");
            Assert.AreSame(userToAssign, CurrentUser.Delegate);
        }


        /// <summary>
        /// Tests the precondition is thrown when currentuser is not in supervisor role.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UCDArch.Core.Utils.PreconditionException))]
        public void TestPreconditionIsThrownWhenCurrentuserIsNotInSupervisorRole()
        {
            try
            {
                const string userToAssignUserName = "UserToAssign";
                _userAuth.Expect(a => a.IsCurrentUserInRole(RoleNames.RoleSupervisor)).Return(false).Repeat.Once();
                _userAuth.RoleProvider
                    .Expect(a => a.IsUserInRole(userToAssignUserName, RoleNames.RoleDelegateSupervisor))
                    .Return(true).Repeat.Any();

                var userToAssign = CreateValidEntities.User(2);
                userToAssign.UserName = userToAssignUserName;

                _delegateBLL.AssignDelegate(userToAssign);
            }
            catch (Exception ex)
            {
                Assert.IsNotNull(ex);
                Assert.AreEqual("Current user must be a supervisor to assign delegates", ex.Message);
                throw;
            }
        }

        #endregion AssignDelegate Tests

        #region RemoveDelegate Tests


        /// <summary>
        /// Tests the remove delegate is saved.
        /// </summary>
        [TestMethod]
        public void TestRemoveDelegateIsSaved()
        {
            const string userToRemoveUserName = "UserToRemove";
            _userAuth.Expect(a => a.IsCurrentUserInRole(RoleNames.RoleSupervisor)).Return(true).Repeat.Once();
            _userAuth.RoleProvider
                .Expect(a => a.IsUserInRole(userToRemoveUserName, RoleNames.RoleDelegateSupervisor))
                .Return(true).Repeat.Once();

            var userToRemove = CreateValidEntities.User(2);
            userToRemove.UserName = userToRemoveUserName;

            CurrentUser.Delegate = userToRemove;

            _users.Add(CurrentUser);
            _users.Add(userToRemove);

            _userBLL.Expect(a => a.Queryable).Return(_users.AsQueryable()).Repeat.Any();

            _delegateBLL.RemoveDelegate(userToRemove);
            _userBLL.AssertWasCalled(a => a.EnsurePersistent(CurrentUser));

            Assert.IsNull(CurrentUser.Delegate);
        }

        #endregion RemoveDelegate Tests

    }
}
