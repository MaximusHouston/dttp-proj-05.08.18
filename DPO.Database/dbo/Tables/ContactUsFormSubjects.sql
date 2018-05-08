CREATE TABLE [dbo].[ContactUsFormSubjects] (
    [SubjectId]   INT           IDENTITY (1, 1) NOT NULL,
    [SubjectName] NVARCHAR (50) NOT NULL,
    CONSTRAINT [PK_ContactUsFormSubjects] PRIMARY KEY CLUSTERED ([SubjectId] ASC) WITH (FILLFACTOR = 99)
);

