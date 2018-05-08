CREATE TABLE [dbo].[States] (
    [StateId]  INT           IDENTITY (1, 1) NOT NULL,
    [CountryCode] CHAR(2) NOT NULL,
    [Name]      NVARCHAR (50) NOT NULL,
    [Code]      NVARCHAR (10) NOT NULL,
    CONSTRAINT [PK_State] PRIMARY KEY CLUSTERED ([StateId] ASC),
    CONSTRAINT [FK_State_Country_States] FOREIGN KEY ([CountryCode]) REFERENCES [dbo].[Countries] ([CountryCode])
);

