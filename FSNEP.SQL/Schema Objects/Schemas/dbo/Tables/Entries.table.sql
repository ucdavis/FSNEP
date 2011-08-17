CREATE TABLE [dbo].[Entries] (
    [ID]               INT           IDENTITY (1, 1) NOT NULL,
    [RecordID]         INT           NOT NULL,
    [FundTypeID]       INT           NULL,
    [ProjectID]        INT           NULL,
    [FinanceAccountID] INT           NULL,
    [Comment]          VARCHAR (256) NULL
);

