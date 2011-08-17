CREATE TABLE [dbo].[Records] (
    [ID]            INT              IDENTITY (1, 1) NOT NULL,
    [Month]         INT              NOT NULL,
    [Year]          INT              NOT NULL,
    [UserId]        UNIQUEIDENTIFIER NOT NULL,
    [StatusID]      INT              NOT NULL,
    [ReviewComment] VARCHAR (512)    NULL
);

