CREATE TABLE [dbo].[QuoteItems] (
    [QuoteItemId]         BIGINT         NOT NULL,
    [QuoteId]             BIGINT         NOT NULL,
    [ProductId]           BIGINT         NULL,
    [ProductNumber]       NVARCHAR (100) NULL,
    [Description]         NVARCHAR (255) NOT NULL,
    [Quantity]            MONEY          NOT NULL,
    [ListPrice]           MONEY          NOT NULL,
    [Multiplier]          MONEY          NOT NULL,
    [AccountMultiplierId] BIGINT         NULL,
    [Timestamp]           DATETIME2 (7)  NOT NULL,
    [Tags]                NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_QuoteItem] PRIMARY KEY CLUSTERED ([QuoteItemId] ASC) WITH (FILLFACTOR = 99),
    CONSTRAINT [FK_QuoteItem_AccountMultiplier_QuoteItems] FOREIGN KEY ([QuoteId]) REFERENCES [dbo].[AccountMultipliers] ([AccountMultiplierId]),
    CONSTRAINT [FK_QuoteItem_Product_QuoteItems] FOREIGN KEY ([ProductId]) REFERENCES [dbo].[Products] ([ProductId]),
    CONSTRAINT [FK_QuoteItem_Quote_QuoteItems] FOREIGN KEY ([QuoteId]) REFERENCES [dbo].[Quotes] ([QuoteId])
);


GO
ALTER TABLE [dbo].[QuoteItems] NOCHECK CONSTRAINT [FK_QuoteItem_AccountMultiplier_QuoteItems];


GO
ALTER TABLE [dbo].[QuoteItems] NOCHECK CONSTRAINT [FK_QuoteItem_Product_QuoteItems];


GO
ALTER TABLE [dbo].[QuoteItems] NOCHECK CONSTRAINT [FK_QuoteItem_AccountMultiplier_QuoteItems]
GO
ALTER TABLE [dbo].[QuoteItems] NOCHECK CONSTRAINT [FK_QuoteItem_Product_QuoteItems]
GO
CREATE NONCLUSTERED INDEX [GMIX_QuoteItem_Cover]
    ON [dbo].[QuoteItems]([QuoteId] ASC, [QuoteItemId] ASC)
    INCLUDE([ProductId], [ProductNumber], [Description], [Quantity], [ListPrice], [Multiplier], [AccountMultiplierId], [Timestamp], [Tags]) WITH (FILLFACTOR = 99);


GO
CREATE STATISTICS [_dta_stat_1205579333_6_2]
    ON [dbo].[QuoteItems]([Quantity], [QuoteId]);

