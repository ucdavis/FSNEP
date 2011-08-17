ALTER TABLE [dbo].[Entries]
    ADD CONSTRAINT [FK_Entries_Projects] FOREIGN KEY ([ProjectID]) REFERENCES [dbo].[Projects] ([ID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

