﻿ALTER TABLE [dbo].[TimeRecords]
    ADD CONSTRAINT [FK_TimeRecords_Records] FOREIGN KEY ([ID]) REFERENCES [dbo].[Records] ([ID]) ON DELETE NO ACTION ON UPDATE NO ACTION;
