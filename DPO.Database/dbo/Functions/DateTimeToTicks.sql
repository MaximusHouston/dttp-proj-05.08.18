
CREATE FUNCTION [dbo].[DateTimeToTicks] (@d datetime2)
RETURNS bigint
WITH SCHEMABINDING
AS
-- converts the given datetime to .NET-compatible ticks
-- see http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpref/html/frlrfsystemdatetimeclasstickstopic.asp
BEGIN 
RETURN 
    dbo.DateToTicks(DATEPART(yyyy, @d), DATEPART(mm, @d), DATEPART(dd, @d)) +
    dbo.TimeToTicks(DATEPART(hh, @d), DATEPART(mi, @d), DATEPART(ss, @d)) +
    (CONVERT(bigint, DATEPART(ms, @d)) * CONVERT(bigint,10000));
END