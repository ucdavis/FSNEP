ALTER TABLE [dbo].[Entries]
    ADD CONSTRAINT [FK_Entries_FinanceAccounts] FOREIGN KEY ([FinanceAccountID]) REFERENCES [dbo].[FinanceAccounts] ([ID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

