CREATE FUNCTION [dbo].[FnProductDocuments] 
(@ProductId BIGINT = NULL,
 @DocumentTypeId INT = NULL) 
RETURNS TABLE 
 
RETURN 
SELECT P.ProductId,P.[Rank],P.[DocumentId],P.[DocumentTypeId], P.[DocumentTypeDescription],P.[FileName],P.[CreatedOn],P.[ModifiedOn],P.[Timestamp] 
FROM VwProductDocuments P 
WHERE P.[DocumentTypeId] = ISNULL(@DocumentTypeId, P.[DocumentTypeId])
AND P.ProductId = ISNULL(@ProductId, P.ProductId)