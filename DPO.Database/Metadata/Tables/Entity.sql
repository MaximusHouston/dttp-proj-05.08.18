CREATE TABLE [Metadata].[Entity] (
    [EntityId]  INT           IDENTITY (1, 1) NOT NULL,
    [Name]      NVARCHAR (80) NOT NULL,
    [TableName] NVARCHAR (80) NULL,
    CONSTRAINT [PK_EntityMetadata] PRIMARY KEY CLUSTERED ([EntityId] ASC)
);

