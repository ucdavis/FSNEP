ALTER TABLE [dbo].[ActivityTypes]
    ADD CONSTRAINT [FK_ActivityTypes_ActivityCategories] FOREIGN KEY ([CategoryID]) REFERENCES [dbo].[ActivityCategories] ([ID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

