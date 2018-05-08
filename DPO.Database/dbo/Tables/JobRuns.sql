CREATE TABLE [dbo].[JobRuns] (
    [JobRunId]    INT           IDENTITY (1, 1) NOT NULL,
    [JobName]     NVARCHAR (80) NOT NULL,
    [LastRunDate] DATETIME      CONSTRAINT [DF_JobRuns_LastRunDate] DEFAULT (getdate()) NOT NULL,
    CONSTRAINT [PK_JobRuns] PRIMARY KEY CLUSTERED ([JobRunId] ASC)
);

