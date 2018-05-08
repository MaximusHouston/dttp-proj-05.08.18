CREATE TABLE [dbo].[ProductSpecifications] (
    [ProductId]                   BIGINT          NOT NULL,
    [ProductSpecificationLabelId] SMALLINT        NOT NULL,
    [Value]                       NVARCHAR (2000) NOT NULL,
    CONSTRAINT [PK_ProductSpecification] PRIMARY KEY CLUSTERED ([ProductId] ASC, [ProductSpecificationLabelId] ASC) WITH (FILLFACTOR = 99),
    CONSTRAINT [FK_ProductSpecification_Product_ProductSpecifications] FOREIGN KEY ([ProductId]) REFERENCES [dbo].[Products] ([ProductId]),
    CONSTRAINT [FK_ProductSpecification_ProductSpecificationLabel_ProductSpecifications] FOREIGN KEY ([ProductSpecificationLabelId]) REFERENCES [dbo].[ProductSpecificationLabels] ([ProductSpecificationLabelId])
);




GO
CREATE NONCLUSTERED INDEX [IX_ProductSpecifications_ProductSpecificationLabelId_ProductId]
    ON [dbo].[ProductSpecifications]([ProductSpecificationLabelId] ASC, [ProductId] ASC)
    INCLUDE([Value]);

