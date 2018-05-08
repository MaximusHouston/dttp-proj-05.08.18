CREATE TABLE [dbo].[ProjectStatusTypes] (
    [ProjectStatusTypeId] TINYINT           NOT NULL,
    [Description]     NVARCHAR (50) NOT NULL,
    CONSTRAINT [PK_ProjectStatusType] PRIMARY KEY CLUSTERED ([ProjectStatusTypeId] ASC)
);

