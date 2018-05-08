CREATE TABLE [dbo].[RequirementLevels] (
    [RequirementLevelId] TINYINT       IDENTITY (1, 1) NOT NULL,
    [Name]               NVARCHAR (50) NOT NULL,
    [Timestamp]          DATETIME2 (7) CONSTRAINT [DF_RequirementLevels_Timestamp] DEFAULT (getdate()) NOT NULL,
    CONSTRAINT [PK_RequirementLevels] PRIMARY KEY CLUSTERED ([RequirementLevelId] ASC)
);

