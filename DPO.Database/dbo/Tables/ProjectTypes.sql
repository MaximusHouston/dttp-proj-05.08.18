CREATE TABLE [dbo].[ProjectTypes] (
    [ProjectTypeId] TINYINT           NOT NULL,
    [Description]   NVARCHAR (50) NOT NULL,
    CONSTRAINT [PK_ProjectType] PRIMARY KEY CLUSTERED ([ProjectTypeId] ASC)
);

