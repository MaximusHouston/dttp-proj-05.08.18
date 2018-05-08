
CREATE FUNCTION [dbo].[DateToTicks] (@year int, @month int, @day int)
RETURNS bigint
WITH SCHEMABINDING
AS
-- converts the given year/month/day to the corresponding ticks
BEGIN 
RETURN CONVERT(bigint, (((((((@year - 1) * 365) + ((@year - 1) / 4)) - ((@year - 1) / 100)) + ((@year - 1) / 400)) + dbo.MonthToDays(@year, @month - 1)) + @day) - 1) * 864000000000;
END