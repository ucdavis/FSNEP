﻿ALTER TABLE [dbo].[Entries]
    ADD CONSTRAINT [FK_Entries_FundTypes] FOREIGN KEY ([FundTypeID]) REFERENCES [dbo].[FundTypes] ([ID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

