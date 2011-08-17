ALTER TABLE [dbo].[RecordTracking]
    ADD CONSTRAINT [FK_RecordTracking_Records] FOREIGN KEY ([RecordID]) REFERENCES [dbo].[Records] ([ID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

