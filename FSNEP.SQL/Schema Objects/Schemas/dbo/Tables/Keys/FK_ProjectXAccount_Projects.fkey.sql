ALTER TABLE [dbo].[ProjectXUsers]
    ADD CONSTRAINT [FK_ProjectXAccount_Projects] FOREIGN KEY ([ProjectID]) REFERENCES [dbo].[Projects] ([ID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

