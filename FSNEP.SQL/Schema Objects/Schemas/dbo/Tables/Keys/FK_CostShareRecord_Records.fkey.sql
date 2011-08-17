ALTER TABLE [dbo].[CostShareRecords]
    ADD CONSTRAINT [FK_CostShareRecord_Records] FOREIGN KEY ([ID]) REFERENCES [dbo].[Records] ([ID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

