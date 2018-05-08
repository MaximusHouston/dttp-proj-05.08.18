
CREATE FUNCTION [dbo].[MonthToDays365] (@month int)
RETURNS int
WITH SCHEMABINDING
AS
-- converts the given month (0-12) to the corresponding number of days into the year (by end of month)
-- this function is for non-leap years
BEGIN 
RETURN
    CASE @month
        WHEN 0 THEN 0
        WHEN 1 THEN 31
        WHEN 2 THEN 59
        WHEN 3 THEN 90
        WHEN 4 THEN 120
        WHEN 5 THEN 151
        WHEN 6 THEN 181
        WHEN 7 THEN 212
        WHEN 8 THEN 243
        WHEN 9 THEN 273
        WHEN 10 THEN 304
        WHEN 11 THEN 334
        WHEN 12 THEN 365
        ELSE 0
    END
END