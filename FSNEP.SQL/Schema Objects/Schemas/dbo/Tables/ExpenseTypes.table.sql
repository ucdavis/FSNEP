﻿CREATE TABLE [dbo].[ExpenseTypes] (
    [ID]       INT          IDENTITY (1, 1) NOT NULL,
    [Name]     VARCHAR (50) NOT NULL,
    [IsActive] BIT          NOT NULL
);

