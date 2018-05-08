CREATE TABLE [dbo].[Addresses] (
    [AddressId]    BIGINT        NOT NULL,
    [AddressLine1] NVARCHAR (50) NULL,
    [AddressLine2] NVARCHAR (50) NULL,
    [AddressLine3] NVARCHAR (50) NULL,
    [Location]     NVARCHAR (50) NULL,
    [StateId]     INT           NULL,
    [PostalCode]   NVARCHAR (10) NULL,
    CONSTRAINT [PK_Address] PRIMARY KEY CLUSTERED ([AddressId] ASC),
    CONSTRAINT [FK_Address_State_Addresses] FOREIGN KEY ([StateId]) REFERENCES [dbo].[States] ([StateId])
);

