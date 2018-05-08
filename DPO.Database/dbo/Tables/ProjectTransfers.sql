CREATE TABLE [dbo].[ProjectTransfers] (
    [UserId]            BIGINT          NOT NULL,
    [ProjectId]         BIGINT          NOT NULL,
	[IsTransferred]		BIT				NOT NULL,
    CONSTRAINT [PK_ProjectTransfer] PRIMARY KEY CLUSTERED ([UserId], [ProjectId]),
    CONSTRAINT [FK_ProjectTransfer_User_ProjectTransfers] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([UserId]), 
	CONSTRAINT [FK_ProjectTransfer_Project_ProjectTransfers] FOREIGN KEY ([ProjectId]) REFERENCES [dbo].[Projects] ([ProjectId]), 
);
GO
CREATE INDEX [IX_ProjectTransfers_Projectid] ON [dbo].[ProjectTransfers] ([ProjectId])