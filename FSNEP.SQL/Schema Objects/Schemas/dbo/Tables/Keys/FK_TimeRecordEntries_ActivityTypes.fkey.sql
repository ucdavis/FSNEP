ALTER TABLE [dbo].[TimeRecordEntries]
    ADD CONSTRAINT [FK_TimeRecordEntries_ActivityTypes] FOREIGN KEY ([ActivityTypeID]) REFERENCES [dbo].[ActivityTypes] ([ID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

