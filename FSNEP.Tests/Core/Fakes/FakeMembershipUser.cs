using System;
using System.Web.Security;

namespace FSNEP.Tests.Core.Fakes
{
    public class FakeMembershipUser : MembershipUser
    {
        public static Guid UserToken = Guid.Empty;

        public override bool ChangePassword(string oldPassword, string newPassword)
        {
            return newPassword.Equals("newPass");
        }

        public override string ResetPassword(string passwordAnswer)
        {
            if (passwordAnswer == "invalidAnswer")
            {
                throw new MembershipPasswordException();
            }

            return "newPassword";
        }

        public override string PasswordQuestion
        {
            get
            {
                return "Question";
            }
        }

        public override object ProviderUserKey
        {
            get
            {
                return UserToken;
            }
        }
    }
}