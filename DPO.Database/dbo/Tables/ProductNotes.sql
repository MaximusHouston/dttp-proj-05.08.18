CREATE TABLE [dbo].[ProductNotes] (
    [ProductNoteId]     UNIQUEIDENTIFIER NOT NULL,
    [ProductId]         BIGINT           NOT NULL,
    [ProductNoteTypeId] INT              NOT NULL,
    [Rank]              SMALLINT         DEFAULT ((0)) NOT NULL,
    [Description]       VARCHAR (1024)   NOT NULL,
    [ModifiedOn]        DATETIME2 (7)    NOT NULL,
    [Timestamp]         DATETIME2 (7)    DEFAULT (getutcdate()) NOT NULL,
    CONSTRAINT [PK_ProductNoteId] PRIMARY KEY CLUSTERED ([ProductNoteId] ASC) WITH (FILLFACTOR = 99),
    CONSTRAINT [FK_Document_ProductNoteType_ProductNotes] FOREIGN KEY ([ProductNoteTypeId]) REFERENCES [dbo].[ProductNoteTypes] ([ProductNoteTypeId]),
    CONSTRAINT [FK_ProductNote_Product_ProductNotes] FOREIGN KEY ([ProductId]) REFERENCES [dbo].[Products] ([ProductId])
);


GO
ALTER TABLE [dbo].[ProductNotes] NOCHECK CONSTRAINT [FK_Document_ProductNoteType_ProductNotes];



GO
CREATE NONCLUSTERED INDEX [IX_ProductNotes]
    ON [dbo].[ProductNotes]([ProductId] ASC);
 
