﻿ALTER TABLE [dbo].[Entries]
    ADD CONSTRAINT [FK_Entries_Records] FOREIGN KEY ([RecordID]) REFERENCES [dbo].[Records] ([ID]) ON DELETE NO ACTION ON UPDATE NO ACTION;
