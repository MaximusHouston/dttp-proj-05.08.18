CREATE TABLE [dbo].[ProductAccessories] (
    [ParentProductId]	BIGINT NOT NULL,
	[ProductId]			BIGINT NOT NULL,
	[Quantity]			INT NOT NULL,
    [RequirementTypeId]	INT  NOT NULL,
	[ModifiedOn]	   DATETIME2      NOT NULL,
    [Timestamp]        DATETIME2       NOT NULL,
    CONSTRAINT [FK_ProductAccessory_ParentProduct_ParentProductAccessories] FOREIGN KEY ([ParentProductId]) REFERENCES [Products]([ProductId]),
	CONSTRAINT [FK_ProductAccessory_Product_ProductAccessories] FOREIGN KEY ([ProductId]) REFERENCES [Products]([ProductId]),
	CONSTRAINT [FK_ProductAccessory_RequirementType_ProductAccessories] FOREIGN KEY ([RequirementTypeId]) REFERENCES [RequirementTypes]([RequirementTypeId]), 
    CONSTRAINT [PK_ProductAccessories] PRIMARY KEY ([ParentProductId], [ProductId]),
);
GO
CREATE NONCLUSTERED INDEX [IX_ProductsAccessories]
    ON [dbo].[ProductAccessories]([ProductId] ASC);
GO