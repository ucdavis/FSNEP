ALTER TABLE [dbo].[ProjectXFinanceAccount]
    ADD CONSTRAINT [FK_ProjectXFinanceAccount_Projects] FOREIGN KEY ([ProjectID]) REFERENCES [dbo].[Projects] ([ID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

