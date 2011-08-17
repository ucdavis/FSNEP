CREATE TABLE [dbo].[TimeRecordEntries] (
    [ID]             INT      NOT NULL,
    [Date]           INT      NOT NULL,
    [Hours]          FLOAT    NOT NULL,
    [ActivityTypeID] INT      NOT NULL,
    [AdjustmentDate] DATETIME NULL
);

