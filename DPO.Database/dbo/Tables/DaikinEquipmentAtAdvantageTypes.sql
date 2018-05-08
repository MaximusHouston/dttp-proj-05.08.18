/*

USE [dbDaikinProjectOfficeLive]
GO

/****** Object:  Table [dbo].[DaikinEquipmentAtAdvantageTypes]    Script Date: 17/02/2015 09:24:36 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[DaikinEquipmentAtAdvantageTypes](
	[DaikinEquipmentAtAdvantageTypeId] [tinyint] NOT NULL,
	[Description] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_DaikinEquipmentAtAdvantageTypes] PRIMARY KEY CLUSTERED 
(
	[DaikinEquipmentAtAdvantageTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING ON
GO

INSERT INTO [dbo].[DaikinEquipmentAtAdvantageTypes]
           ([DaikinEquipmentAtAdvantageTypeId]
           ,[Description])
     VALUES
           (1, 'Advantage'),
		   (2, 'Disadvantage'),
		   (3, 'Neutral')
GO


*/