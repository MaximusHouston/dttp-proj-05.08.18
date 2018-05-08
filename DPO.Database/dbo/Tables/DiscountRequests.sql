CREATE TABLE [dbo].[DiscountRequests] (
    [DiscountRequestId]                BIGINT         NOT NULL,
    [ProjectId]                        BIGINT         NOT NULL,
    [QuoteId]                          BIGINT         NOT NULL,
    [StartUpCosts]                     MONEY          NOT NULL,
    [RequestedDiscount]                MONEY          NOT NULL,
    [RequestedCommission]              MONEY          NOT NULL,
    [RequestedOn]                      DATE           NOT NULL,
    [DiscountRequestStatusTypeId]      TINYINT        NOT NULL,
    [StatusTypeModifiedOn]             DATE           NOT NULL,
    [StatusTypeModifiedBy]             BIGINT         NULL,
    [SystemBasisDesignTypeId]          TINYINT        NULL,
    [ZoneStrategyTypeId]               TINYINT        NULL,
    [BrandSpecifiedTypeId]             TINYINT        NULL,
    [BrandApprovedTypeId]              TINYINT        NULL,
    [HasCompetitorPrice]               BIT            NOT NULL,
    [CompetitorPrice]                  MONEY          NULL,
    [HasCompetitorQuote]               BIT            NOT NULL,
    [CompetitorQuoteFileName]          VARCHAR (255)  NULL,
    [HasCompetitorLineComparsion]      BIT            NOT NULL,
    [CompetitorLineComparsionFileName] VARCHAR (255)  NULL,
    [IsConfidentCompetitorQuote]       BIT            NOT NULL,
    [ApprovalAssuresOrder]             BIT            NOT NULL,
    [OrderPlannedFor]                  DATE           NULL,
    [OrderDeliveryDate]                DATE           NULL,
    [Notes]                            NVARCHAR (MAX) NULL,
    [ResponseNotes]                    NVARCHAR (MAX) NULL,
    [Timestamp]                        DATETIME2 (7)  NOT NULL,
    [DaikinEquipmentAtAdvantageTypeId] TINYINT        NULL,
    [ProbabilityOfCloseTypeId]         TINYINT        NULL,
    [ApprovedDiscount]                 MONEY          NULL,
    CONSTRAINT [PK_DiscountRequest] PRIMARY KEY CLUSTERED ([DiscountRequestId] ASC) WITH (FILLFACTOR = 99),
    CONSTRAINT [FK_DiscountRequest_BrandApprovedType_DiscountRequestsBrandApproved] FOREIGN KEY ([BrandApprovedTypeId]) REFERENCES [dbo].[BrandCompetitorTypes] ([BrandCompetitorTypeId]),
    CONSTRAINT [FK_DiscountRequest_BrandSpecifiedType_DiscountRequestsBrandCompetitor] FOREIGN KEY ([BrandSpecifiedTypeId]) REFERENCES [dbo].[BrandCompetitorTypes] ([BrandCompetitorTypeId]),
    CONSTRAINT [FK_DiscountRequest_DiscountRequestStatusType_DiscountRequests] FOREIGN KEY ([DiscountRequestStatusTypeId]) REFERENCES [dbo].[DiscountRequestStatusTypes] ([DiscountRequestStatusTypeId]),
    CONSTRAINT [FK_DiscountRequest_Quote_DiscountRequests] FOREIGN KEY ([QuoteId]) REFERENCES [dbo].[Quotes] ([QuoteId]),
    CONSTRAINT [FK_DiscountRequest_SystemBasisDesignType_DiscountRequests] FOREIGN KEY ([SystemBasisDesignTypeId]) REFERENCES [dbo].[SystemBasisDesignTypes] ([SystemBasisDesignTypeId]),
    CONSTRAINT [FK_DiscountRequest_ZoneStrategyType_DiscountRequests] FOREIGN KEY ([ZoneStrategyTypeId]) REFERENCES [dbo].[ZoneStrategyTypes] ([ZoneStrategyTypeId])
);








GO
CREATE NONCLUSTERED INDEX [IX_DiscountRequests_QuoteId]
    ON [dbo].[DiscountRequests]([QuoteId] ASC);

