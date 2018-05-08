CREATE TABLE [dbo].[Links] (
    [LinkId]       BIGINT         IDENTITY (1, 1) NOT NULL,
    [LinkCategory] NVARCHAR (100) NOT NULL,
    [LinkName]     NVARCHAR (255) NOT NULL,
    [LinkUrl]      NVARCHAR (255) NOT NULL,
    [Order]        INT            CONSTRAINT [DF_Links_Order] DEFAULT ((0)) NOT NULL,
    [Enabled]      BIT            CONSTRAINT [DF_Links_Active] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_Links] PRIMARY KEY CLUSTERED ([LinkId] ASC) WITH (FILLFACTOR = 99)
);

