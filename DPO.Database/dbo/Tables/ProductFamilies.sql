CREATE TABLE [dbo].[ProductFamilies] (
    [ProductFamilyId] INT NOT NULL,
	[Name]		      NVARCHAR (50)     NOT NULL,
    [Description]     NVARCHAR (512)    NULL,
    [Timestamp] DATETIME2 NOT NULL, 
    CONSTRAINT [PK_DaikinProductFamily] PRIMARY KEY CLUSTERED ([ProductFamilyId] ASC)
);

