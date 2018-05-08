CREATE TABLE [dbo].[ContactUsFormOptions] (
    [ContactUsFormId] INT            IDENTITY (1, 1) NOT NULL,
    [Address]         NVARCHAR (100) NULL,
    [Telephone]       NVARCHAR (50)  NULL,
    CONSTRAINT [PK_ContactUsFormOptionss] PRIMARY KEY CLUSTERED ([ContactUsFormId] ASC) WITH (FILLFACTOR = 99)
);

