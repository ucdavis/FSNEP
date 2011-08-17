ALTER TABLE [dbo].[CostShareRecordEntries]
    ADD CONSTRAINT [FK_CostShareRecordEntries_ExpenseTypes] FOREIGN KEY ([ExpenseTypeID]) REFERENCES [dbo].[ExpenseTypes] ([ID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

