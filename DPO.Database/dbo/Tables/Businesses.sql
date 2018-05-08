CREATE TABLE [dbo].[Businesses] (
    [BusinessId]              BIGINT         NOT NULL,
    [BusinessName]            NVARCHAR (50)  NOT NULL,
    [AccountId]               NVARCHAR (50)  NULL,
    [AddressId]               BIGINT         NULL,
    [ShipToAddressId]         BIGINT         NULL,
    [ContactId]               BIGINT         NULL,
    [BusinessTypeId]          INT            NOT NULL,
    [CommissionSchemeAllowed] BIT            NOT NULL,
    [ShowPricing]             BIT            NOT NULL,
    [YearToDateSales]         MONEY          NOT NULL,
    [OpenOrdersTotal]         MONEY          NOT NULL,
    [Enabled]                 BIT            NOT NULL,
    [DaikinModifiedOn]        DATETIME2 (7)  NOT NULL,
    [Timestamp]               DATETIME2 (7)  NOT NULL,
    [AccountManagerEmail]     NVARCHAR (200) NULL,
    [AccountOwnerEmail]       NVARCHAR (200) NULL,
    [AccountManagerFirstName] NVARCHAR (100) NULL,
    [AccountManagerLastName]  NVARCHAR (100) NULL,
    [AccountOwnerFirstName]   NVARCHAR (100) NULL,
    [AccountOwnerLastName]    NVARCHAR (100) NULL,
    [AccountOwningGroupName]  NVARCHAR (200) NULL,
    [ERPAccountId]            NVARCHAR (25)  NULL,
    CONSTRAINT [PK_Business] PRIMARY KEY CLUSTERED ([BusinessId] ASC) WITH (FILLFACTOR = 99),
    CONSTRAINT [FK_Business_Address_Businesses] FOREIGN KEY ([AddressId]) REFERENCES [dbo].[Addresses] ([AddressId]),
    CONSTRAINT [FK_Business_BusinessType_Businesses] FOREIGN KEY ([BusinessTypeId]) REFERENCES [dbo].[BusinessTypes] ([BusinessTypeId]),
    CONSTRAINT [FK_Business_Contact_Businesses] FOREIGN KEY ([ContactId]) REFERENCES [dbo].[Contacts] ([ContactId])
);



