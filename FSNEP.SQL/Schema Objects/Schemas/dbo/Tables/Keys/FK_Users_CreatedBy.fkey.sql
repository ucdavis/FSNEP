﻿ALTER TABLE [dbo].[Users]
    ADD CONSTRAINT [FK_Users_CreatedBy] FOREIGN KEY ([CreatedByID]) REFERENCES [dbo].[Users] ([UserId]) ON DELETE NO ACTION ON UPDATE NO ACTION;

