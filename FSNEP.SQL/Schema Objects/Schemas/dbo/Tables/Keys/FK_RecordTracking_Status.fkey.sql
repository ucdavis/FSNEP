ALTER TABLE [dbo].[RecordTracking]
    ADD CONSTRAINT [FK_RecordTracking_Status] FOREIGN KEY ([StatusID]) REFERENCES [dbo].[Status] ([ID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

