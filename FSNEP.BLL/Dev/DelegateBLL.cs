using System.Linq;
using FSNEP.BLL.Impl;
using FSNEP.BLL.Interfaces;
using FSNEP.Core.Abstractions;
using FSNEP.Core.Domain;
using UCDArch.Core.Utils;

namespace FSNEP.BLL.Dev
{
    public class DelegateBLL : IDelegateBLL {
        private readonly IUserAuth _userAuth;
        private readonly IUserBLL _userBLL;

        public DelegateBLL(IUserAuth userAuth, IUserBLL userBLL)
        {
            _userAuth = userAuth;
            _userBLL = userBLL;
        }

        public void AssignDelegate(User userToAssign)
        {
            var userToAssignName = userToAssign.UserName;

            Check.Require(_userAuth.IsCurrentUserInRole(RoleNames.RoleSupervisor),
                          "Current user must be a supervisor to assign delegates");

            //Assign the new delegate user the delegate role if they don't already have it
            var roleProvider = _userAuth.RoleProvider;

            if (roleProvider.IsUserInRole(userToAssignName, RoleNames.RoleDelegateSupervisor) == false)
            {
                roleProvider.AddUsersToRoles(new[] {userToAssignName}, new[] {RoleNames.RoleDelegateSupervisor});
            }

            var currentUser = _userBLL.GetUser();

            currentUser.Delegate = userToAssign;

            _userBLL.EnsurePersistent(currentUser);
        }

        public void RemoveDelegate(User userToRemove)
        {
            var currentUser = _userBLL.GetUser();

            Check.Require(currentUser.Delegate == userToRemove, "You can only remove your current delegate");

            //Remove the delegate user from the delegate role if they aren't anyone else's delegate
            var isDelegateOfOtherUser =
                _userBLL.Queryable.Where(user => user.Delegate.Id == userToRemove.Id && user.Id != currentUser.Id).Any();

            if (isDelegateOfOtherUser == false)
            {
                var roleProvider = _userAuth.RoleProvider;

                roleProvider.RemoveUsersFromRoles(new[] {userToRemove.UserName},
                                                  new[] {RoleNames.RoleDelegateSupervisor});
            }

            currentUser.Delegate = null;

            _userBLL.EnsurePersistent(currentUser);
        }
    }
}