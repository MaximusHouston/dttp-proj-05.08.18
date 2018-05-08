CREATE TABLE [dbo].[DecisionTreeDependancies] (
    [id]          INT IDENTITY (1, 1) NOT NULL,
    [ConfigId]    INT NOT NULL,
    [SystemIndex] INT NOT NULL,
    [index]       INT NOT NULL,
    CONSTRAINT [PK_DecisionTreeDependancies_1] PRIMARY KEY CLUSTERED ([id] ASC) WITH (FILLFACTOR = 99)
);

