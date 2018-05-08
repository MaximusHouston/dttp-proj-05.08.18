CREATE TABLE [dbo].[LibraryDirectories] (
    [LibraryDirectoryId] INT           IDENTITY (1, 1) NOT NULL,
    [ParentId]           INT           NULL,
    [Name]               VARCHAR (MAX) NOT NULL,
    [Protected]          BIT           NOT NULL,
    CONSTRAINT [PK_LibraryDirectories] PRIMARY KEY CLUSTERED ([LibraryDirectoryId] ASC) WITH (FILLFACTOR = 99)
);

