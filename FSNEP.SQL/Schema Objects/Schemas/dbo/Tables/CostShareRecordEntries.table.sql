CREATE TABLE [dbo].[CostShareRecordEntries] (
    [ID]            INT           NOT NULL,
    [ExpenseTypeID] INT           NOT NULL,
    [ExpenseAmount] FLOAT         NOT NULL,
    [EntryFileID]   INT           NULL,
    [Description]   VARCHAR (128) NOT NULL,
    [Exclude]       BIT           NOT NULL,
    [ExcludeReason] VARCHAR (256) NULL
);

