ALTER TABLE [dbo].[CostShareRecordEntries]
    ADD CONSTRAINT [FK_CostShareRecordEntries_Entries] FOREIGN KEY ([ID]) REFERENCES [dbo].[Entries] ([ID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

