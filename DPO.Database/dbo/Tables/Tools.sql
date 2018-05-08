CREATE TABLE [dbo].[Tools] (
    [ToolId]      INT            NOT NULL,
    [Name]        NVARCHAR (50)  NOT NULL,
    [Timestamp]   DATETIME2 (7)  NOT NULL,
    [Filename]    NCHAR (255)    NULL,
    [Description] NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_Tool] PRIMARY KEY CLUSTERED ([ToolId] ASC) WITH (FILLFACTOR = 99)
);



