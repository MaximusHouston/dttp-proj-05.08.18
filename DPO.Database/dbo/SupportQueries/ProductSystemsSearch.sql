/*
SELECT  *
FROM  Products p
INNER JOIN [dbo].[ProductAccessories] pa ON p.ProductId = pa.ParentProductId
INNER JOIN Products a ON pa.ProductId = a.ProductId
INNER JOIN ProductModelTypes pt ON pt.ProductModelTypeId = a.ProductModelTypeId 
WHERE pt.Description in ('indoor','outdoor')
and p.ProductNumber in('REYQ312PBYD','REYQ312PBTJ')
*/