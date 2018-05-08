CREATE TABLE [dbo].[BuildingFloors] (
    [BuildingId]      INT           NOT NULL,
    [FloorId]         INT           IDENTITY (1, 1) NOT NULL,
    [Name]            VARCHAR (MAX) NOT NULL,
    [TypeId]          INT           NOT NULL,
    [size]            VARCHAR (50)  NULL,
    [FloorImage]      VARCHAR (MAX) NULL,
    [ApplicationId]   INT           NULL,
    [BackgroundImage] VARCHAR (MAX) NULL,
    [Icon]            VARCHAR (MAX) NULL,
    CONSTRAINT [PK_BuildingFloors] PRIMARY KEY CLUSTERED ([FloorId] ASC) WITH (FILLFACTOR = 99)
);

