
CREATE FUNCTION [dbo].[MonthToDays366] (@month int)
RETURNS int 
WITH SCHEMABINDING
AS
-- converts the given month (0-12) to the corresponding number of days into the year (by end of month)
-- this function is for leap years
BEGIN 
RETURN
    CASE @month
        WHEN 0 THEN 0
        WHEN 1 THEN 31
        WHEN 2 THEN 60
        WHEN 3 THEN 91
        WHEN 4 THEN 121
        WHEN 5 THEN 152
        WHEN 6 THEN 182
        WHEN 7 THEN 213
        WHEN 8 THEN 244
        WHEN 9 THEN 274
        WHEN 10 THEN 305
        WHEN 11 THEN 335
        WHEN 12 THEN 366
        ELSE 0
    END
END