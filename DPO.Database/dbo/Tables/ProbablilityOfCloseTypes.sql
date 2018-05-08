/***

USE [dbDaikinProjectOfficeLive]
GO

/****** Object:  Table [dbo].[ProbablilityOfCloseTypes]    Script Date: 17/02/2015 09:25:53 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[ProbablilityOfCloseTypes](
	[ProbablilityOfCloseTypeId] [tinyint] NOT NULL,
	[Description] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_ProbablilityOfCloseTypes] PRIMARY KEY CLUSTERED 
(
	[ProbablilityOfCloseTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING ON
GO

INSERT INTO [dbo].[ProbablilityOfCloseTypes]
           ([ProbablilityOfCloseTypeId]
           ,[Description])
     VALUES
           (1,'10%'),(2,'20%'),(3,'30%'),(4,'40%'),(5,'50%'),(6,'60%'),(7,'70%'),(8,'80%'),(9,'90%'),(10,'100%')
GO


***/