﻿ALTER TABLE [dbo].[ProjectXUsers]
    ADD CONSTRAINT [PK_ProjectXUsers] PRIMARY KEY CLUSTERED ([ProjectID] ASC, [UserId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);
