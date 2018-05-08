CREATE TABLE [dbo].[CommsCenterVideos] (
    [VideoId]   TINYINT IDENTITY (1, 1) NOT NULL,
    [Title]     TEXT    NOT NULL,
    [Thumbnail] TEXT    NOT NULL,
    [Link]      TEXT    NOT NULL,
    CONSTRAINT [PK_CommsCenterVideos] PRIMARY KEY CLUSTERED ([VideoId] ASC) WITH (FILLFACTOR = 99)
);

