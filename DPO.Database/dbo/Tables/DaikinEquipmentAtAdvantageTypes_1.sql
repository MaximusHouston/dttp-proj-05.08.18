CREATE TABLE [dbo].[DaikinEquipmentAtAdvantageTypes] (
    [DaikinEquipmentAtAdvantageTypeId] TINYINT       NOT NULL,
    [Description]                      NVARCHAR (50) NOT NULL,
    CONSTRAINT [PK_DaikinEquipmentAtAdvantageTypes] PRIMARY KEY CLUSTERED ([DaikinEquipmentAtAdvantageTypeId] ASC) WITH (FILLFACTOR = 99)
);

