CREATE TABLE [dbo].[ProductSpecificationLabels] (
	[ProductSpecificationLabelId]  SMALLINT NOT NULL,
    [Name]  NVARCHAR (50) NOT NULL,
    [Description] NVARCHAR (2000)  NULL,
	CONSTRAINT [PK_ProductSpecificationLabels] PRIMARY KEY CLUSTERED ([ProductSpecificationLabelId] ASC)


);

