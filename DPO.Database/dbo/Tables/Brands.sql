CREATE TABLE [dbo].[Brands] (
    [BrandId]     INT        NOT NULL,
    [Name] NVARCHAR (50) NOT NULL,
    [Timestamp] DATETIME2 NOT NULL, 
    CONSTRAINT [PK_Brand] PRIMARY KEY CLUSTERED ([BrandId] ASC)
);

