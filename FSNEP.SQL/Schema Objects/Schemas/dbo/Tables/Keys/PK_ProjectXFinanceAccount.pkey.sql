﻿ALTER TABLE [dbo].[ProjectXFinanceAccount]
    ADD CONSTRAINT [PK_ProjectXFinanceAccount] PRIMARY KEY CLUSTERED ([ProjectID] ASC, [FinanceAccountID] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);
