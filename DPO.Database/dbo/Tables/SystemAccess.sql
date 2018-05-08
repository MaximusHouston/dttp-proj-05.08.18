CREATE TABLE [dbo].[SystemAccess] (
    [SystemAccessId]     INT        NOT NULL,
    [Name] NVARCHAR (50) NOT NULL,
    [Timestamp]   DATETIME2      NOT NULL,
    CONSTRAINT [PK_SystemAccess] PRIMARY KEY CLUSTERED ([SystemAccessId] ASC)
);
