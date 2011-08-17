ALTER TABLE [dbo].[Users]
    ADD CONSTRAINT [FK_Users_Supervisor] FOREIGN KEY ([SupervisorID]) REFERENCES [dbo].[Users] ([UserId]) ON DELETE NO ACTION ON UPDATE NO ACTION;

