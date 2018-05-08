CREATE TABLE [dbo].[LibraryDocumentRelationships] (
    [Id]                 INT IDENTITY (1, 1) NOT NULL,
    [LibraryDocumentId]  INT NOT NULL,
    [LibraryDirectoryId] INT NOT NULL,
    CONSTRAINT [PK_LibraryDocumentRelationships] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (FILLFACTOR = 99),
    CONSTRAINT [FK_LibraryDocumentRelationships_LibraryDirectories] FOREIGN KEY ([LibraryDirectoryId]) REFERENCES [dbo].[LibraryDirectories] ([LibraryDirectoryId]) ON DELETE CASCADE,
    CONSTRAINT [FK_LibraryDocumentRelationships_LibraryDirectories1] FOREIGN KEY ([LibraryDocumentId]) REFERENCES [dbo].[LibraryDocuments] ([LibraryDocumentId]) ON DELETE CASCADE
);

