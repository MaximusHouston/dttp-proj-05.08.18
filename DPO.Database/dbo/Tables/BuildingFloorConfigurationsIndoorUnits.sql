CREATE TABLE [dbo].[BuildingFloorConfigurationsIndoorUnits] (
    [Id]              INT IDENTITY (1, 1) NOT NULL,
    [ConfigurationId] INT NOT NULL,
    [SystemId]        INT NOT NULL,
    CONSTRAINT [PK_BuildingFloorConfigurationsIndoorUnits2] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (FILLFACTOR = 99)
);

