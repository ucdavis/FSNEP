CREATE TABLE [dbo].[ActivityTypes] (
    [ID]         INT          IDENTITY (1, 1) NOT NULL,
    [Name]       VARCHAR (50) NOT NULL,
    [Indicator]  CHAR (2)     NOT NULL,
    [CategoryID] INT          NULL,
    [IsActive]   BIT          NOT NULL
);

