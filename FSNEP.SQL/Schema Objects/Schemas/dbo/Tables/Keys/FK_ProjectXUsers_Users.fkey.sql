ALTER TABLE [dbo].[ProjectXUsers]
    ADD CONSTRAINT [FK_ProjectXUsers_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([UserId]) ON DELETE NO ACTION ON UPDATE NO ACTION;

