CREATE TABLE [dbo].[ProductModelTypes] (
    [ProductModelTypeId] INT           NOT NULL,
    [Description]     NVARCHAR (50) NOT NULL,
    CONSTRAINT [PK_ProductModelType] PRIMARY KEY CLUSTERED ([ProductModelTypeId] ASC)
);

