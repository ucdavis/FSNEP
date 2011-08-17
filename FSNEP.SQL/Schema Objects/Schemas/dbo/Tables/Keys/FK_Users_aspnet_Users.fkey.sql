ALTER TABLE [dbo].[Users]
    ADD CONSTRAINT [FK_Users_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId]) ON DELETE NO ACTION ON UPDATE NO ACTION;

