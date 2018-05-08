CREATE TABLE [dbo].[CityAreas] (
    [CityAreaId]  INT        NOT NULL,
    [Name] NVARCHAR (50) NOT NULL,
    [Timestamp] DATETIME2 NOT NULL, 
    CONSTRAINT [PK_CityArea] PRIMARY KEY CLUSTERED ([CityAreaId] ASC)
);

