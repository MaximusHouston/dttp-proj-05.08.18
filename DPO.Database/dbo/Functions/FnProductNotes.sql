CREATE FUNCTION [dbo].[FnProductNotes] 
(@ProductId BIGINT = NULL,
 @ProductNoteTypeId INT = NULL) 
RETURNS TABLE 
 
RETURN 
SELECT * FROM VwProductNotes V 
WHERE V.[ProductNoteTypeId] = ISNULL(@ProductNoteTypeId, V.[ProductNoteTypeId])
AND V.ProductId = ISNULL(@ProductId, V.ProductId)