CREATE TABLE [dbo].[BrandCompetitorTypes] (
    [BrandCompetitorTypeId] TINYINT           NOT NULL,
    [Description]         NVARCHAR (50) NOT NULL,
    CONSTRAINT [PK_BrandCompetitorType] PRIMARY KEY CLUSTERED ([BrandCompetitorTypeId] ASC)
);

