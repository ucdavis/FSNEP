ALTER TABLE [dbo].[Users]
    ADD CONSTRAINT [DF_Users_resetPassword] DEFAULT ((1)) FOR [ResetPassword];

