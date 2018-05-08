CREATE TABLE [dbo].[Users] (
    [UserId]             BIGINT          NOT NULL,
    [GroupId]            BIGINT          NOT NULL,
    [IsGroupOwner]       BIT             NULL,
    [FirstName]          NVARCHAR (50)   NOT NULL,
    [MiddleName]         NVARCHAR (50)   NULL,
    [LastName]           NVARCHAR (50)   NOT NULL,
    [Email]              NVARCHAR (50)   NOT NULL,
    [Password]           NVARCHAR (128)  NOT NULL,
    [Salt]               INT             NOT NULL,
    [UserTypeId]         TINYINT         NOT NULL,
    [UseBusinessAddress] BIT             NOT NULL,
    [BusinessId]         BIGINT          NULL,
    [AddressId]          BIGINT          NULL,
    [ContactId]          BIGINT          NULL,
    [Approved]           BIT             CONSTRAINT [DF__Users__Approved__6477ECF3] DEFAULT ((0)) NOT NULL,
    [Rejected]           BIT             CONSTRAINT [DF__Users__Rejected__656C112C] DEFAULT ((0)) NOT NULL,
    [Enabled]            BIT             CONSTRAINT [DF__Users__Enabled__66603565] DEFAULT ((0)) NOT NULL,
    [RegisteredOn]       DATETIME2 (7)   NULL,
    [ApprovedOn]         DATETIME2 (7)   NULL,
    [ShowPricing]        BIT             NOT NULL,
    [LastLoginOn]        DATETIME2 (7)   NULL,
    [DisplaySettings]    NVARCHAR (1024) NULL,
    [Timestamp]          DATETIME2 (7)   NOT NULL,
    CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED ([UserId] ASC) WITH (FILLFACTOR = 99),
    CONSTRAINT [FK_User_Address_Users] FOREIGN KEY ([AddressId]) REFERENCES [dbo].[Addresses] ([AddressId]),
    CONSTRAINT [FK_User_Business_Users] FOREIGN KEY ([BusinessId]) REFERENCES [dbo].[Businesses] ([BusinessId]),
    CONSTRAINT [FK_User_Contact_Users] FOREIGN KEY ([ContactId]) REFERENCES [dbo].[Contacts] ([ContactId]),
    CONSTRAINT [FK_User_Group_Users] FOREIGN KEY ([GroupId]) REFERENCES [dbo].[Groups] ([GroupId]),
    CONSTRAINT [FK_User_UserType_Users] FOREIGN KEY ([UserTypeId]) REFERENCES [dbo].[UserTypes] ([UserTypeId])
);




GO


CREATE INDEX [IX_Users_Groupid] ON [dbo].[Users] ([GroupId])
GO
CREATE UNIQUE INDEX [IX_Users_Email] ON [dbo].[Users] ([Email])
GO
CREATE NONCLUSTERED INDEX [IX_Users_BusinessId]
    ON [dbo].[Users]([BusinessId] ASC)
    INCLUDE([GroupId]);


GO
CREATE NONCLUSTERED INDEX [GMIX_Users_Cover]
    ON [dbo].[Users]([GroupId] ASC, [BusinessId] ASC, [UserId] ASC, [UserTypeId] ASC, [IsGroupOwner] ASC, [FirstName] ASC, [MiddleName] ASC, [LastName] ASC, [Email] ASC, [Password] ASC, [Salt] ASC, [UseBusinessAddress] ASC, [AddressId] ASC, [ContactId] ASC, [Approved] ASC, [Rejected] ASC)
    INCLUDE([Enabled], [RegisteredOn], [ApprovedOn], [ShowPricing], [LastLoginOn], [DisplaySettings], [Timestamp]) WITH (FILLFACTOR = 99);


GO
CREATE STATISTICS [_dta_stat_654625375_2_1_10_4_6]
    ON [dbo].[Users]([GroupId], [UserId], [UserTypeId], [FirstName], [LastName]);


GO
CREATE STATISTICS [_dta_stat_654625375_1_4_6_10]
    ON [dbo].[Users]([UserId], [FirstName], [LastName], [UserTypeId]);


GO
CREATE STATISTICS [_dta_stat_654625375_1_23]
    ON [dbo].[Users]([UserId], [Timestamp]);


GO
CREATE STATISTICS [_dta_stat_654625375_1_10_4]
    ON [dbo].[Users]([UserId], [UserTypeId], [FirstName]);

