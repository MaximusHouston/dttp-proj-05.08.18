
CREATE FUNCTION [dbo].[MonthToDays] (@year int, @month int)
RETURNS int
WITH SCHEMABINDING
AS
-- converts the given month (0-12) to the corresponding number of days into the year (by end of month)
-- this function is for non-leap years
BEGIN 
RETURN 
    -- determine whether the given year is a leap year
    CASE 
        WHEN (@year % 4 = 0) and ((@year % 100  != 0) or ((@year % 100 = 0) and (@year % 400 = 0))) THEN dbo.MonthToDays366(@month)
        ELSE dbo.MonthToDays365(@month)
    END
END