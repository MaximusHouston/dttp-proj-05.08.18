CREATE FUNCTION [dbo].[FnProductSpecifications] 
(
@ProductId BIGINT = NULL
) 
RETURNS TABLE 
 
RETURN 
SELECT * FROM VwProductSpecifications V 
WHERE 
V.ProductId = ISNULL(@ProductId, V.ProductId)