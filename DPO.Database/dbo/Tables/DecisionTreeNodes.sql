CREATE TABLE [dbo].[DecisionTreeNodes] (
    [NodeId]       INT IDENTITY (1, 1) NOT NULL,
    [SystemId]     INT NOT NULL,
    [ParentNodeId] INT NULL,
    [LayoutId]     INT NULL,
    CONSTRAINT [PK_DecisionTreeNodes] PRIMARY KEY CLUSTERED ([NodeId] ASC) WITH (FILLFACTOR = 99)
);

