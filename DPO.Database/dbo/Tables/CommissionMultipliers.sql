CREATE TABLE [dbo].[CommissionMultipliers] (
    [Multiplier] MONEY        NOT NULL,
    [CommissionPercentage]   MONEY        NOT NULL, 
    CONSTRAINT [PK_Multipliers] PRIMARY KEY ([Multiplier])
);

