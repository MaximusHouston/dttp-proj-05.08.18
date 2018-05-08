CREATE TABLE [dbo].[ProductCategories]
(
    [ProductCategoryId] INT NOT NULL,
	[Name]		      NVARCHAR (50)     NOT NULL,
    [Description]     NVARCHAR (512)    NULL,
    CONSTRAINT [PK_DaikinProductCategory] PRIMARY KEY CLUSTERED ([ProductCategoryId] ASC)
);

