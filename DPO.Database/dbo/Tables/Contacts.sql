CREATE TABLE [dbo].[Contacts] (
    [ContactId]    BIGINT        NOT NULL,
    [Phone]        NVARCHAR (50) NULL,
    [Mobile]       NVARCHAR (50) NULL,
    [ContactEmail] NVARCHAR (50) NULL,
    [Website]      NVARCHAR (50) NULL,
    CONSTRAINT [PK_Contact] PRIMARY KEY CLUSTERED ([ContactId] ASC)
);

