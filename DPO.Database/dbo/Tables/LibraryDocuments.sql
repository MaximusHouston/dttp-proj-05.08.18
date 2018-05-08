CREATE TABLE [dbo].[LibraryDocuments] (
    [LibraryDocumentId] INT           IDENTITY (1, 1) NOT NULL,
    [Name]              VARCHAR (MAX) NOT NULL,
    [Path]              VARCHAR (MAX) NULL,
    [Thumb]             VARCHAR (MAX) NULL,
    [Timestamp]         DATETIME2 (7) CONSTRAINT [DF_LibraryDocuments_Timestamp] DEFAULT (getdate()) NOT NULL,
    CONSTRAINT [PK_LibraryDocument] PRIMARY KEY CLUSTERED ([LibraryDocumentId] ASC) WITH (FILLFACTOR = 99)
);

