CREATE TABLE [dbo].[PermissionTypes] (
    [PermissionTypeId]   TINYINT        NOT NULL,
    [Name]               NVARCHAR (50)  NOT NULL,
    [Description]        NVARCHAR (255) NULL,
    [ReferenceTableName] NVARCHAR (100) NOT NULL,
    CONSTRAINT [PK_PermissionTypeId] PRIMARY KEY CLUSTERED ([PermissionTypeId] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_PermissionTypes]
    ON [dbo].[PermissionTypes]([PermissionTypeId] ASC);

