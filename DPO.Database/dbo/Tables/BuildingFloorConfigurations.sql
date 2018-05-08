CREATE TABLE [dbo].[BuildingFloorConfigurations] (
    [FloorConfigId] INT           IDENTITY (1, 1) NOT NULL,
    [Name]          VARCHAR (MAX) NOT NULL,
    [SystemName]    VARCHAR (MAX) NOT NULL,
    [SystemSize]    VARCHAR (MAX) NOT NULL,
    [Energy]        VARCHAR (50)  NOT NULL,
    [FloorId]       INT           NOT NULL,
    [OverlayImage]  VARCHAR (MAX) NULL,
    [SystemImage]   VARCHAR (MAX) NULL,
    [SystemType]    VARCHAR (MAX) NOT NULL,
    [Alternate]     BIT           NULL,
    CONSTRAINT [PK_BuildingFloorConfgurations] PRIMARY KEY CLUSTERED ([FloorConfigId] ASC) WITH (FILLFACTOR = 99)
);

