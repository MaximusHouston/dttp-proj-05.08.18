CREATE TABLE [dbo].[ProjectExportTypes] (
    [ProjectExportTypeId] TINYINT        NOT NULL,
    [Description]         NVARCHAR (200) NOT NULL,
    CONSTRAINT [PK_ProjectExportType] PRIMARY KEY CLUSTERED ([ProjectExportTypeId] ASC) WITH (FILLFACTOR = 99)
);

