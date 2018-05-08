CREATE TABLE [dbo].[BusinessTypes] (
    [BusinessTypeId]               INT           NOT NULL,
    [Description]                  NVARCHAR (50) NOT NULL,
    [BusinessIdRequirementLevelId] TINYINT       NULL,
    CONSTRAINT [PK_BusinessType] PRIMARY KEY CLUSTERED ([BusinessTypeId] ASC) WITH (FILLFACTOR = 99),
    CONSTRAINT [FK_BusinessTypes_RequirementLevels] FOREIGN KEY ([BusinessIdRequirementLevelId]) REFERENCES [dbo].[RequirementLevels] ([RequirementLevelId])
);



