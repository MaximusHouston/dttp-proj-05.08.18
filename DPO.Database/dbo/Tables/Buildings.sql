CREATE TABLE [dbo].[Buildings] (
    [BuildingId]         INT            NOT NULL,
    [Name]               NVARCHAR (MAX) NOT NULL,
    [TypeId]             INT            NOT NULL,
    [VideoIn]            NVARCHAR (MAX) NOT NULL,
    [VideoInPoster]      NVARCHAR (MAX) NULL,
    [HotSpotX]           NCHAR (10)     NOT NULL,
    [HotSpotY]           NCHAR (10)     NOT NULL,
    [MenuImage]          NVARCHAR (MAX) NULL,
    [BuildingFolderName] NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_Buildings] PRIMARY KEY CLUSTERED ([BuildingId] ASC) WITH (FILLFACTOR = 99)
);

