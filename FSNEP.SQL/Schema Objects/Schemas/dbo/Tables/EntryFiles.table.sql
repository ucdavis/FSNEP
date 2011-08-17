CREATE TABLE [dbo].[EntryFiles] (
    [ID]          INT             IDENTITY (1, 1) NOT NULL,
    [Content]     VARBINARY (MAX) NOT NULL,
    [ContentType] VARCHAR (50)    NULL,
    [Name]        VARCHAR (512)   NOT NULL,
    [DateAdded]   DATETIME        NOT NULL
);

