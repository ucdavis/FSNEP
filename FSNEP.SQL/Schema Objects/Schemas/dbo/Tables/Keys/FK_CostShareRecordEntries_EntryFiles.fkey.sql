ALTER TABLE [dbo].[CostShareRecordEntries]
    ADD CONSTRAINT [FK_CostShareRecordEntries_EntryFiles] FOREIGN KEY ([EntryFileID]) REFERENCES [dbo].[EntryFiles] ([ID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

