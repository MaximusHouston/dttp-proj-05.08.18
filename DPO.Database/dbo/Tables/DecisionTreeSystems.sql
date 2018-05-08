CREATE TABLE [dbo].[DecisionTreeSystems] (
    [id]       INT IDENTITY (1, 1) NOT NULL,
    [ConfigId] INT NOT NULL,
    [SystemId] INT NOT NULL,
    [index]    INT NOT NULL,
    CONSTRAINT [PK_DecisionTreeSystems] PRIMARY KEY CLUSTERED ([id] ASC) WITH (FILLFACTOR = 99)
);

