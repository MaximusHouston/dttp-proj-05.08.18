CREATE TABLE [dbo].[UserTypes] (
    [UserTypeId] TINYINT       NOT NULL,
	[Description]   NVARCHAR (50) NOT NULL 
    CONSTRAINT [PK_UserTypeId] PRIMARY KEY CLUSTERED ([UserTypeId] ASC )    
);

