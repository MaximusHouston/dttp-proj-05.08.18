CREATE TABLE [dbo].[VerticalMarketTypes]
(
	VerticalMarketTypeId TINYINT NOT NULL,
	[Description]    NVARCHAR (50) NOT NULL 
    CONSTRAINT [PK_VerticalMarketType] PRIMARY KEY CLUSTERED ([VerticalMarketTypeId] ASC) 
)
 