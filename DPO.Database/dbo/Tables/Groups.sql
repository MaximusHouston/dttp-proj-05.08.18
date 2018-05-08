CREATE TABLE [dbo].[Groups] (
    [GroupId]		BIGINT			NOT NULL,
	[ParentGroupId]	BIGINT			NULL,
    [Name]			NVARCHAR (50)	NOT NULL,
	[DaikinGroupId] UNIQUEIDENTIFIER NULL,
	[Path]			NVARCHAR (512)	NOT NULL,
	[ChildrenCount]		SMALLINT		NOT NULL,
	[ChildrenCountDeep]	SMALLINT		NOT NULL,
	[MemberCount]		SMALLINT		NOT NULL,
    [Timestamp]     DATETIME2 NOT NULL, 
    CONSTRAINT [PK_Group] PRIMARY KEY CLUSTERED ([GroupId] ASC),
);
	GO


