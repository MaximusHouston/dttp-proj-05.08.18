CREATE TABLE [dbo].[Countries] (
    [CountryCode] CHAR(2) NOT NULL,
    [Name]      NVARCHAR (50) NOT NULL,
    [ISO3]      CHAR(3) NULL,
    [PhoneCode] INT           NULL,
    CONSTRAINT [PK_Country] PRIMARY KEY CLUSTERED ([CountryCode] ASC)
);

