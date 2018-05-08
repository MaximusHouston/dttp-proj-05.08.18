CREATE TABLE [dbo].[Products] (
    [ProductId]             BIGINT          NOT NULL,
    [ProductNumber]         NVARCHAR (100)  NOT NULL,
    [Name]                  NVARCHAR (100)  NOT NULL,
    [ProductClassCode]      NVARCHAR (50)   NOT NULL,
    [ProductFamilyId]       INT             NOT NULL,
    [ProductCategoryId]     INT             NOT NULL,
    [BrandId]               INT             NOT NULL,
    [ProductMarketTypeId]   INT             NOT NULL,
    [ProductModelTypeId]    INT             NOT NULL,
    [SubmittalSheetTypeId]  INT             NOT NULL,
    [ProductStatusId]       INT             CONSTRAINT [DF__Products__Produc__60A75C0F] DEFAULT ((1)) NOT NULL,
    [AllowCommissionScheme] BIT             NOT NULL,
    [ListPrice]             MONEY           NOT NULL,
    [ModifiedOn]            DATETIME2 (7)   NOT NULL,
    [PowerVoltage]          INT             NULL,
    [HeatingCapacityRated]  DECIMAL (18, 4) NULL,
    [CoolingCapacityRated]  DECIMAL (18, 4) NULL,
    [EERNonducted]          DECIMAL (18, 4) NULL,
    [IEERNonDucted]         DECIMAL (18, 4) NULL,
    [SEERNonducted]         DECIMAL (18, 4) NULL,
    [COP47Nonducted]        DECIMAL (18, 4) NULL,
    [HSPFNonducted]         DECIMAL (18, 4) NULL,
    [Timestamp]             DATETIME2 (7)   NOT NULL,
    CONSTRAINT [PK_Product] PRIMARY KEY CLUSTERED ([ProductId] ASC) WITH (FILLFACTOR = 99),
    CONSTRAINT [FK_Product_Brand_Products] FOREIGN KEY ([BrandId]) REFERENCES [dbo].[Brands] ([BrandId]),
    CONSTRAINT [FK_Product_ProductCategory_Products] FOREIGN KEY ([ProductCategoryId]) REFERENCES [dbo].[ProductCategories] ([ProductCategoryId]),
    CONSTRAINT [FK_Product_ProductFamily_Products] FOREIGN KEY ([ProductFamilyId]) REFERENCES [dbo].[ProductFamilies] ([ProductFamilyId]),
    CONSTRAINT [FK_Product_ProductMarket_Products] FOREIGN KEY ([ProductMarketTypeId]) REFERENCES [dbo].[ProductMarketTypes] ([ProductMarketTypeId]),
    CONSTRAINT [FK_Product_ProductModel_Products] FOREIGN KEY ([ProductModelTypeId]) REFERENCES [dbo].[ProductModelTypes] ([ProductModelTypeId]),
    CONSTRAINT [FK_Product_SubmittialSheet_Products] FOREIGN KEY ([SubmittalSheetTypeId]) REFERENCES [dbo].[SubmittalSheetTypes] ([SubmittalSheetTypeId])
);



GO
CREATE NONCLUSTERED INDEX [IX_Products]
    ON [dbo].[Products]([ProductFamilyId] ASC);
GO
CREATE NONCLUSTERED INDEX [IX_Products_1]
    ON [dbo].[Products]([BrandId] ASC);
	GO
CREATE NONCLUSTERED INDEX [IX_Products_2]
    ON [dbo].[Products]([ProductNumber] ASC);
	GO
CREATE NONCLUSTERED INDEX [IX_Products_3]
    ON [dbo].[Products]([AllowCommissionScheme] ASC);

