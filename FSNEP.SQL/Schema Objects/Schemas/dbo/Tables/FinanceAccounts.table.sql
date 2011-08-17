CREATE TABLE [dbo].[FinanceAccounts] (
    [ID]           INT          IDENTITY (1, 1) NOT NULL,
    [Name]         VARCHAR (50) NOT NULL,
    [IndirectCost] FLOAT        NOT NULL,
    [IsActive]     BIT          NOT NULL
);

