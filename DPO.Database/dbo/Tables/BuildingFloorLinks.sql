CREATE TABLE [dbo].[BuildingFloorLinks] (
    [LinkId]  INT            IDENTITY (1, 1) NOT NULL,
    [FloorId] INT            NOT NULL,
    [Title]   NVARCHAR (MAX) NOT NULL,
    [Copy]    NVARCHAR (MAX) NOT NULL,
    [Url]     NVARCHAR (MAX) NOT NULL,
    [Enabled] BIT            NOT NULL,
    CONSTRAINT [PK_FloorLinks] PRIMARY KEY CLUSTERED ([LinkId] ASC) WITH (FILLFACTOR = 99)
);

