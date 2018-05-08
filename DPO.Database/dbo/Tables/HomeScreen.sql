CREATE TABLE [dbo].[HomeScreen] (
    [HomeScreenId]  TINYINT NOT NULL,
    [Title]         TEXT    NOT NULL,
    [BodyText]      TEXT    NOT NULL,
    [PrivacyPolicy] TEXT    NOT NULL,
    CONSTRAINT [PK_HomeScreen] PRIMARY KEY CLUSTERED ([HomeScreenId] ASC) WITH (FILLFACTOR = 99)
);

