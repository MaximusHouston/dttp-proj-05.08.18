CREATE TABLE [dbo].[BillboardPosters] (
    [PosterId] TINYINT NOT NULL,
    [Url]      TEXT    NOT NULL,
    [Image]    TEXT    NOT NULL,
    [enabled]  BIT     NOT NULL,
    CONSTRAINT [PK_BillboardPosters] PRIMARY KEY CLUSTERED ([PosterId] ASC) WITH (FILLFACTOR = 99)
);

