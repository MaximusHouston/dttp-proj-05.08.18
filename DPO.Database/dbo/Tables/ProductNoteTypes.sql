CREATE TABLE [dbo].[ProductNoteTypes] (
    [ProductNoteTypeId] INT           NOT NULL,
    [Description]     NVARCHAR (50) NOT NULL,
    CONSTRAINT [PK_ProductNoteTypeId] PRIMARY KEY CLUSTERED ([ProductNoteTypeId] ASC)
);

