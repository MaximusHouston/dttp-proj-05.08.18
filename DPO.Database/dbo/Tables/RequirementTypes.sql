CREATE TABLE [dbo].[RequirementTypes] (
    [RequirementTypeId]  INT           NOT NULL,
    [Description]         NVARCHAR (50) NOT NULL, 
    CONSTRAINT [PK_RequirementType] PRIMARY KEY CLUSTERED ([RequirementTypeId] ASC)
);

