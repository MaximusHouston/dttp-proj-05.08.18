CREATE TABLE [dbo].[Quotes] (
    [QuoteId]                      BIGINT          NOT NULL,
    [Revision]                     INT             NOT NULL,
    [ProjectId]                    BIGINT          NOT NULL,
    [Title]                        NVARCHAR (255)  NOT NULL,
    [Description]                  NVARCHAR (1000) NULL,
    [Multiplier]                   MONEY           NOT NULL,
    [DiscountPercentage]           MONEY           CONSTRAINT [DF__Quotes__Discount__6383C8BA] DEFAULT ((0)) NOT NULL,
    [RecalculationRequired]        BIT             NOT NULL,
    [AwaitingDiscountRequest]      BIT             CONSTRAINT [DF_Quotes_AwaitingDiscountRequest] DEFAULT ((0)) NOT NULL,
    [Notes]                        NVARCHAR (MAX)  NULL,
    [Active]                       BIT             NOT NULL,
    [TotalCountCommission]         TINYINT         NOT NULL,
    [TotalCountNonCommission]      TINYINT         NOT NULL,
    [TotalCountService]            TINYINT         NOT NULL,
    [TotalCountMisc]               TINYINT         NOT NULL,
    [TotalListCommission]          MONEY           NOT NULL,
    [TotalListNonCommission]       MONEY           NOT NULL,
    [TotalListService]             MONEY           NOT NULL,
    [TotalList]                    MONEY           NOT NULL,
    [TotalNetCommission]           MONEY           NOT NULL,
    [TotalNetNonCommission]        MONEY           NOT NULL,
    [TotalNetService]              MONEY           NOT NULL,
    [TotalNet]                     MONEY           CONSTRAINT [DF_Quotes_TotalNet] DEFAULT ((0)) NOT NULL,
    [TotalMisc]                    MONEY           NOT NULL,
    [TotalSell]                    MONEY           NOT NULL,
    [TotalService]                 MONEY           CONSTRAINT [DF_Quotes_TotalService] DEFAULT ((0)) NOT NULL,
    [TotalFreight]                 MONEY           NOT NULL,
    [IsCommissionScheme]           BIT             CONSTRAINT [DF__Quotes__IsCommis__619B8048] DEFAULT ((0)) NOT NULL,
    [IsGrossMargin]                BIT             CONSTRAINT [DF__Quotes__IsGrossM__628FA481] DEFAULT ((0)) NOT NULL,
    [CommissionPercentage]         MONEY           NOT NULL,
    [DiscountRequestId]            BIGINT          NULL,
    [ApprovedDiscountPercentage]   MONEY           CONSTRAINT [DF_Quotes_ApprovedDiscountPercentage] DEFAULT ((0)) NOT NULL,
    [ApprovedCommissionPercentage] MONEY           CONSTRAINT [DF_Quotes_ApprovedCommissionPercentage] DEFAULT ((0)) NOT NULL,
    [VRVOutdoorCount]              INT             CONSTRAINT [DF_Quotes_VRVOutdoorCount] DEFAULT ((0)) NOT NULL,
    [SplitCount]                   INT             CONSTRAINT [DF_Quotes_SplitCount] DEFAULT ((0)) NOT NULL,
    [RTUCount]                     INT             CONSTRAINT [DF_Quotes_RTUCount] DEFAULT ((0)) NOT NULL,
    [VRVIndoorCount]               INT             CONSTRAINT [DF_Quotes_VRVIndoorCount] DEFAULT ((0)) NOT NULL,
    [Deleted]                      BIT             NOT NULL,
    [CreatedDate]                  DATE            NOT NULL,
    [Timestamp]                    DATETIME2 (7)   NOT NULL,
    CONSTRAINT [PK_Quote] PRIMARY KEY CLUSTERED ([QuoteId] ASC) WITH (FILLFACTOR = 99),
    CONSTRAINT [FK_Quote_Project_Quotes] FOREIGN KEY ([ProjectId]) REFERENCES [dbo].[Projects] ([ProjectId])
);








GO
CREATE NONCLUSTERED INDEX [IX_Quotes_RecalculationRequired]
    ON [dbo].[Quotes]([RecalculationRequired] ASC)
    INCLUDE([QuoteId]);


GO
CREATE NONCLUSTERED INDEX [GMIX_Quote_Cover]
    ON [dbo].[Quotes]([ProjectId] ASC, [Active] ASC)
    INCLUDE([QuoteId], [Revision], [Title], [RecalculationRequired], [Timestamp], [AwaitingDiscountRequest], [TotalList], [TotalNet], [TotalMisc], [TotalSell], [DiscountRequestId]) WITH (FILLFACTOR = 99);

