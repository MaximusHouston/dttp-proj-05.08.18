CREATE TABLE [dbo].[ProductSpecificationKeyLookups] (
	[ProductSpecificationLabelId]		SMALLINT NOT NULL,
	[Key]		INT NOT NULL,
    [Value] VARCHAR(255) NOT NULL,
    CONSTRAINT [PK_ProductSpecificationKeyLookups] PRIMARY KEY CLUSTERED ([ProductSpecificationLabelId]	 ASC,[Key] ASC),
	CONSTRAINT [FK_ProductSpecificationKeyLookups_ProductSpecificationLabel_ProductSpecificationKeyLookups] FOREIGN KEY ([ProductSpecificationLabelId]) REFERENCES ProductSpecificationLabels([ProductSpecificationLabelId])

);

