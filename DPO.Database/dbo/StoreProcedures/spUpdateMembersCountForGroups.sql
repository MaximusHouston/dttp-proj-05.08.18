CREATE PROCEDURE [dbo].[spUpdateMembersCountForGroups]
AS
	UPDATE dbo.Groups
	SET MemberCount = ISNULL(S.MemberCount,0)
	--SELECT *
	FROM dbo.Groups G
	LEFT JOIN 
	(
		SELECT GroupId,MemberCount=Count(*) FROM dbo.Users GROUP BY GroupId
	) S ON G.GroupId = S.GroupId
	WHERE ISNULL(S.MemberCount,0) != G.MemberCount

