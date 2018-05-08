CREATE TABLE [dbo].[UserBasketItems] (
	[BasketItemId]      BIGINT         NOT NULL,
    [UserId]            BIGINT         NOT NULL,
    [ItemId]            BIGINT         NOT NULL,
	[Description]       VARCHAR(255)   NOT NULL,
    [Quantity]          MONEY  NOT NULL, 
	CONSTRAINT [PK_UserBasketItem] PRIMARY KEY ([BasketItemId]),
	CONSTRAINT [FK_UserBasketItem_User_UserBasketItems] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users]([UserId])

);
GO
CREATE NONCLUSTERED INDEX [IX_User_Basket]
    ON [dbo].[UserBasketItems]([UserId] ASC);
	GO