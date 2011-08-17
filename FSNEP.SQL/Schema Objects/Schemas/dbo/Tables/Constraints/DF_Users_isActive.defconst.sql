ALTER TABLE [dbo].[Users]
    ADD CONSTRAINT [DF_Users_isActive] DEFAULT ((1)) FOR [IsActive];

