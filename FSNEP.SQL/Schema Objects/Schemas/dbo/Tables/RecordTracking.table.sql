CREATE TABLE [dbo].[RecordTracking] (
    [ID]               INT             IDENTITY (1, 1) NOT NULL,
    [RecordID]         INT             NOT NULL,
    [StatusID]         INT             NOT NULL,
    [ActionDate]       DATETIME        NOT NULL,
    [UserName]         NVARCHAR (256)  NOT NULL,
    [DigitalSignature] VARBINARY (MAX) NULL
);

