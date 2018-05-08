
CREATE VIEW [dbo].[VwProductSpecifications]
AS 
SELECT 
	P.ProductId,SpecificationId = PSL.ProductSpecificationLabelId,
	SpecificationName = PSL.Name, 
	SpecificationKey = PSK.[Key],
	Value = ISNULL(PSK.Value,PS.Value)
FROM [dbo].[ProductSpecifications] PS
INNER JOIN [dbo].[ProductSpecificationLabels] PSL ON PSL.ProductSpecificationLabelId = PS.ProductSpecificationLabelId
INNER JOIN [dbo].[Products] P ON P.ProductId = PS.ProductId
LEFT JOIN [dbo].[ProductSpecificationKeyLookups] PSK ON PSK.ProductSpecificationLabelId = PS.ProductSpecificationLabelId 
AND CONVERT(VARCHAR(50),PSK.[Key]) = PS.Value
