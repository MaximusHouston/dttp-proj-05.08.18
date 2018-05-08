CREATE TABLE [dbo].[SubmittalSheetTypes] (
    [SubmittalSheetTypeId] INT           NOT NULL,
    [Description]         NVARCHAR (50) NOT NULL, 
    CONSTRAINT [PK_SubmittalSheetType] PRIMARY KEY CLUSTERED ([SubmittalSheetTypeId] ASC)
);

