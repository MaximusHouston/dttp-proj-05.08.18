CREATE FUNCTION [dbo].[FnGetGroupsAboveGroupId] 
(@GroupId BIGINT) 
RETURNS TABLE 
 
RETURN 
WITH GroupsCTE (GroupId,ParentGroupId)
AS
(
-- Anchor member definition
    SELECT GroupId, ParentGroupId FROM Groups  WHERE GroupId  = @GroupId
	UNION ALL
-- Recursive member definition
	SELECT c.GroupId, c.ParentGroupId FROM Groups c 
	INNER JOIN GroupsCTE cte on cte.ParentGroupId = c.GroupId
)
-- Statement that executes the CTE
SELECT g.GroupId, g.ParentGroupId, g.Name, g.ChildrenCount,g.ChildrenCountDeep,g.MemberCount, g.DaikinGroupId,g.[Path],g.[Timestamp] FROM GroupsCTE
inner join Groups g on g.GroupId =  GroupsCTE.GroupId