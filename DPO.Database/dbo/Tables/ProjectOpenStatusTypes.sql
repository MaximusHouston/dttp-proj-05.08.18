CREATE TABLE [dbo].[ProjectOpenStatusTypes] (
    [ProjectOpenStatusTypeId] TINYINT       NOT NULL,
    [Description]             NVARCHAR (50) NOT NULL,
    [Order]                   TINYINT       CONSTRAINT [DF_ProjectOpenStatusTypes_Order] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_ProjectOpenStatusType] PRIMARY KEY CLUSTERED ([ProjectOpenStatusTypeId] ASC) WITH (FILLFACTOR = 99)
);



