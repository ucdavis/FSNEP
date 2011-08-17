ALTER TABLE [dbo].[Audits]
    ADD CONSTRAINT [FK_Audits_ActionCodes1] FOREIGN KEY ([AuditActionTypeId]) REFERENCES [dbo].[AuditActionTypes] ([ID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

