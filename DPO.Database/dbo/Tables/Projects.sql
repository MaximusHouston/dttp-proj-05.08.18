CREATE TABLE [dbo].[Projects] (
    [ProjectId]               BIGINT          NOT NULL,
    [OwnerId]                 BIGINT          NOT NULL,
    [Name]                    NVARCHAR (255)  NOT NULL,
    [Description]             NVARCHAR (1000) NULL,
    [CustomerName]            NVARCHAR (255)  NULL,
    [EngineerName]            NVARCHAR (255)  NULL,
    [SellerName]              NVARCHAR (255)  NULL,
    [ShipToName]              NVARCHAR (255)  NULL,
    [ProjectDate]             DATE            NOT NULL,
    [BidDate]                 DATE            NOT NULL,
    [EstimatedClose]          DATE            NOT NULL,
    [EstimatedDelivery]       DATE            NOT NULL,
    [Expiration]              DATE            NOT NULL,
    [ProjectOpenStatusTypeId] TINYINT         NOT NULL,
    [ConstructionTypeId]      TINYINT         NOT NULL,
    [ProjectTypeId]           TINYINT         NOT NULL,
    [ProjectStatusTypeId]     TINYINT         NOT NULL,
    [VerticalMarketTypeId]    TINYINT         NOT NULL,
    [EngineerAddressId]       BIGINT          NULL,
    [SellerAddressId]         BIGINT          NULL,
    [CustomerAddressId]       BIGINT          NULL,
    [ShipToAddressId]         BIGINT          NULL,
    [ActiveVersion]           INT             NOT NULL,
    [Deleted]                 BIT             NOT NULL,
    [Timestamp]               DATETIME2 (7)   NOT NULL,
    [DealerContractorName]    NVARCHAR (255)  NULL,
    [ProjectStatusNotes] NVARCHAR(500) NULL, 
    CONSTRAINT [PK_Project] PRIMARY KEY CLUSTERED ([ProjectId] ASC) WITH (FILLFACTOR = 99),
    CONSTRAINT [FK_Project_ConstructionType_Projects] FOREIGN KEY ([ConstructionTypeId]) REFERENCES [dbo].[ConstructionTypes] ([ConstructionTypeId]),
    CONSTRAINT [FK_Project_CustomerAddress_CustomerProjects] FOREIGN KEY ([CustomerAddressId]) REFERENCES [dbo].[Addresses] ([AddressId]),
    CONSTRAINT [FK_Project_EngineerAddress_EngineerProjects] FOREIGN KEY ([EngineerAddressId]) REFERENCES [dbo].[Addresses] ([AddressId]),
    CONSTRAINT [FK_Project_Owner_Projects] FOREIGN KEY ([OwnerId]) REFERENCES [dbo].[Users] ([UserId]),
    CONSTRAINT [FK_Project_ProjectOpenStatusType_Projects] FOREIGN KEY ([ProjectOpenStatusTypeId]) REFERENCES [dbo].[ProjectOpenStatusTypes] ([ProjectOpenStatusTypeId]),
    CONSTRAINT [FK_Project_ProjectStatusType_Projects] FOREIGN KEY ([ProjectStatusTypeId]) REFERENCES [dbo].[ProjectStatusTypes] ([ProjectStatusTypeId]),
    CONSTRAINT [FK_Project_ProjectType_Projects] FOREIGN KEY ([ProjectTypeId]) REFERENCES [dbo].[ProjectTypes] ([ProjectTypeId]),
    CONSTRAINT [FK_Project_SellerAddress_SellerProjects] FOREIGN KEY ([SellerAddressId]) REFERENCES [dbo].[Addresses] ([AddressId]),
    CONSTRAINT [FK_Project_ShipToAddress_ShipToProjects] FOREIGN KEY ([ShipToAddressId]) REFERENCES [dbo].[Addresses] ([AddressId]),
    CONSTRAINT [FK_Project_VerticalMarketType_Projects] FOREIGN KEY ([VerticalMarketTypeId]) REFERENCES [dbo].[VerticalMarketTypes] ([VerticalMarketTypeId])
);


GO
ALTER TABLE [dbo].[Projects] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = ON);




GO
CREATE NONCLUSTERED INDEX [IX_Projects_OwnerId_ProjectStatusTypeId_Deleted_Expiration]
    ON [dbo].[Projects]([OwnerId] ASC, [ProjectStatusTypeId] ASC, [Deleted] ASC, [Expiration] ASC)
    INCLUDE([ProjectId], [Name], [Description], [CustomerName], [EngineerName], [SellerName], [ShipToName], [ProjectDate], [BidDate], [EstimatedClose], [EstimatedDelivery], [ProjectOpenStatusTypeId], [ConstructionTypeId], [ProjectTypeId], [VerticalMarketTypeId], [EngineerAddressId], [SellerAddressId], [CustomerAddressId], [ShipToAddressId], [ActiveVersion], [Timestamp]);


GO
CREATE NONCLUSTERED INDEX [IX_Projects_OwnerId]
    ON [dbo].[Projects]([OwnerId] ASC)
    INCLUDE([ProjectId], [Name], [Description], [CustomerName], [EngineerName], [SellerName], [ShipToName], [ProjectDate], [BidDate], [EstimatedClose], [EstimatedDelivery], [Expiration], [ProjectOpenStatusTypeId], [ConstructionTypeId], [ProjectTypeId], [ProjectStatusTypeId], [VerticalMarketTypeId], [EngineerAddressId], [SellerAddressId], [CustomerAddressId], [ShipToAddressId], [ActiveVersion], [Deleted], [Timestamp]);


GO
CREATE NONCLUSTERED INDEX [GMIX_Projects_ProjectType]
    ON [dbo].[Projects]([ProjectTypeId] ASC) WITH (FILLFACTOR = 99);


GO
CREATE NONCLUSTERED INDEX [GMIX_Projects_ProjectStatus]
    ON [dbo].[Projects]([ProjectStatusTypeId] ASC) WITH (FILLFACTOR = 99);


GO
CREATE NONCLUSTERED INDEX [GMIX_Projects_ProjectOpenStatus]
    ON [dbo].[Projects]([ProjectOpenStatusTypeId] ASC) WITH (FILLFACTOR = 99);


GO
CREATE NONCLUSTERED INDEX [GMIX_Projects_Cover]
    ON [dbo].[Projects]([ProjectDate] ASC, [Deleted] ASC, [OwnerId] ASC, [ProjectId] ASC, [Name] ASC)
    INCLUDE([Description], [CustomerName], [EngineerName], [SellerName], [ShipToName], [BidDate], [EstimatedClose], [EstimatedDelivery], [Expiration], [ProjectOpenStatusTypeId], [ConstructionTypeId], [ProjectTypeId], [ProjectStatusTypeId], [VerticalMarketTypeId], [EngineerAddressId], [SellerAddressId], [CustomerAddressId], [ShipToAddressId], [ActiveVersion], [Timestamp]) WITH (FILLFACTOR = 99);


GO
CREATE STATISTICS [_dta_stat_1234103437_1_25]
    ON [dbo].[Projects]([ProjectId], [Timestamp]);

