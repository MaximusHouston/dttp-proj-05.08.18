CREATE TABLE [dbo].[ConstructionTypes] (
    [ConstructionTypeId] TINYINT           NOT NULL,
    [Description]   NVARCHAR (50) NOT NULL,
    CONSTRAINT [PK_ConstructionTypes] PRIMARY KEY CLUSTERED ([ConstructionTypeId] ASC) 
);

