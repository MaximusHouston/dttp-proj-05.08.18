
CREATE FUNCTION [dbo].[TimeToTicks] (@hour int, @minute int, @second int)  
RETURNS bigint 
WITH SCHEMABINDING
AS 
-- converts the given hour/minute/second to the corresponding ticks
BEGIN 
RETURN (((@hour * 3600) + CONVERT(bigint, @minute) * 60) + CONVERT(bigint, @second)) * 10000000
END