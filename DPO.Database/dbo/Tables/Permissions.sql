CREATE TABLE [dbo].[Permissions] (
    [PermissionId]       BIGINT  NOT NULL,
    [ParentPermissionId] BIGINT  NULL,
    [ObjectEntityId]     INT     NULL,
    [ObjectId]           BIGINT  NOT NULL,
    [PermissionTypeId]   TINYINT NOT NULL,
    [ReferenceEntityId]  INT     NULL,
    [ReferenceId]        INT     NOT NULL,
    CONSTRAINT [PK_Permission] PRIMARY KEY CLUSTERED ([PermissionId] ASC) WITH (FILLFACTOR = 99),
    CONSTRAINT [FK_Permissions_PermissionTypes] FOREIGN KEY ([PermissionTypeId]) REFERENCES [dbo].[PermissionTypes] ([PermissionTypeId])
);







GO
CREATE NONCLUSTERED INDEX [IX_Permissions_1]
    ON [dbo].[Permissions]([ObjectId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Permissions]
    ON [dbo].[Permissions]([PermissionTypeId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Permissions_PermissionTypeId]
    ON [dbo].[Permissions]([PermissionTypeId] ASC)
    INCLUDE([PermissionId], [ParentPermissionId], [ObjectId], [ReferenceId]);

