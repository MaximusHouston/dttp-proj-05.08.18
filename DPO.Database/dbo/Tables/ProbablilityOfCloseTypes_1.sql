CREATE TABLE [dbo].[ProbablilityOfCloseTypes] (
    [ProbablilityOfCloseTypeId] TINYINT       NOT NULL,
    [Description]               NVARCHAR (50) NOT NULL,
    CONSTRAINT [PK_ProbablilityOfCloseTypes] PRIMARY KEY CLUSTERED ([ProbablilityOfCloseTypeId] ASC) WITH (FILLFACTOR = 99)
);

