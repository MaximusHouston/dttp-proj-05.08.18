CREATE FUNCTION [dbo].[FnGetPermissionsUnderPermissionId] 
 (@PermissionId BIGINT) 
 RETURNS TABLE 
 
RETURN 
WITH PermissionsCTE (PermissionId, ParentPermissionId)
AS
(
-- Anchor member definition
    SELECT PermissionId, ParentPermissionId FROM Permissions  WHERE PermissionId  = @PermissionId
	UNION ALL
-- Recursive member definition
	SELECT c.PermissionId, c.ParentPermissionId FROM Permissions c 
	INNER JOIN PermissionsCTE cte on cte.PermissionId = c.ParentPermissionId
)
-- Statement that executes the CTE
SELECT p.* FROM PermissionsCTE
inner join Permissions p on p.PermissionId = PermissionsCTE.PermissionId