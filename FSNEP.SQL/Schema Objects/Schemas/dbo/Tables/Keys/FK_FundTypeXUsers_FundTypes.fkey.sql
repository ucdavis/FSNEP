﻿ALTER TABLE [dbo].[FundTypeXUsers]
    ADD CONSTRAINT [FK_FundTypeXUsers_FundTypes] FOREIGN KEY ([FundTypeID]) REFERENCES [dbo].[FundTypes] ([ID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

