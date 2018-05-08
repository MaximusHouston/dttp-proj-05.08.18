CREATE TABLE [dbo].[CitySystems] (
    [SystemId]    INT           IDENTITY (1, 1) NOT NULL,
    [Name]        VARCHAR (MAX) NOT NULL,
    [Description] VARCHAR (MAX) NOT NULL,
    [Image]       VARCHAR (MAX) NOT NULL,
    [Icon]        VARCHAR (MAX) NOT NULL,
    [Deleted]     BIT           NULL,
    CONSTRAINT [PK_CitySystems] PRIMARY KEY CLUSTERED ([SystemId] ASC) WITH (FILLFACTOR = 99)
);

