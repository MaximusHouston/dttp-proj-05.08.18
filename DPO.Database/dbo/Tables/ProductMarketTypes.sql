CREATE TABLE [dbo].[ProductMarketTypes] (
    [ProductMarketTypeId] INT           NOT NULL,
    [Description]     NVARCHAR (50) NOT NULL,
    CONSTRAINT [PK_ProductMarketType] PRIMARY KEY CLUSTERED ([ProductMarketTypeId] ASC)
);

