CREATE TABLE [dbo].[AccountMultipliers] (
    [AccountMultiplierId] BIGINT        NOT NULL,
    [BusinessId]          BIGINT        NOT NULL,
    [ProductClassCode]    NVARCHAR (50) NOT NULL,
    [Multiplier]          MONEY         NOT NULL,
    [Timestamp]           DATETIME2 (7) NOT NULL,
    CONSTRAINT [PK_AccountMultiplier] PRIMARY KEY CLUSTERED ([AccountMultiplierId] ASC) WITH (FILLFACTOR = 99),
    CONSTRAINT [FK_AccountMultiplier_Business_AccountMultipliers] FOREIGN KEY ([BusinessId]) REFERENCES [dbo].[Businesses] ([BusinessId])
);




GO
CREATE NONCLUSTERED INDEX [IX_AccountMultipliers_BusinessId_ProductClassCode]
    ON [dbo].[AccountMultipliers]([BusinessId] ASC, [ProductClassCode] ASC);

