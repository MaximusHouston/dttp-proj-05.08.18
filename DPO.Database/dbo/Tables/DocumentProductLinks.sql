CREATE TABLE [dbo].[DocumentProductLinks] (
    [ProductId]  BIGINT           NOT NULL,
    [DocumentId] UNIQUEIDENTIFIER NOT NULL,
    [Rank]       SMALLINT         CONSTRAINT [DF__DocumentPr__Rank__5CD6CB2B] DEFAULT ((0)) NOT NULL,
    [Timestamp]  DATETIME2 (7)    DEFAULT (getutcdate()) NOT NULL,
    CONSTRAINT [PK_ProductIdDocumentId] PRIMARY KEY CLUSTERED ([ProductId] ASC, [DocumentId] ASC) WITH (FILLFACTOR = 99),
    CONSTRAINT [FK_DocumentProductLink_Document_Products] FOREIGN KEY ([DocumentId]) REFERENCES [dbo].[Documents] ([DocumentId]),
    CONSTRAINT [FK_DocumentProductLink_Product_Documents] FOREIGN KEY ([ProductId]) REFERENCES [dbo].[Products] ([ProductId])
);


 
