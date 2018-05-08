CREATE TABLE [dbo].[DocumentTypes] (
    [DocumentTypeId] INT           NOT NULL,
    [Description]     NVARCHAR (50) NOT NULL,
    CONSTRAINT [PK_DocumentTypeId] PRIMARY KEY CLUSTERED ([DocumentTypeId] ASC)
);

