ALTER TABLE [dbo].[ProjectXFinanceAccount]
    ADD CONSTRAINT [FK_ProjectXFinanceAccount_FinanceAccounts] FOREIGN KEY ([FinanceAccountID]) REFERENCES [dbo].[FinanceAccounts] ([ID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

