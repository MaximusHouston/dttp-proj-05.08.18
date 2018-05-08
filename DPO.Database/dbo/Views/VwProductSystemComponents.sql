CREATE VIEW [dbo].[VwProductSystemComponents]
AS 

SELECT 

ProductId			= p.ProductId, 
ListPrice			= p.ListPrice, 
ComponentProductId	= a.ProductId, 
ComponentQuantity	= pa.Quantity, 
ComponentListPrice	= a.ListPrice,
ComponentModelTypeId= a.ProductModelTypeId

FROM  Products p
INNER JOIN [dbo].[ProductAccessories] pa ON p.ProductId = pa.ParentProductId
INNER JOIN Products a ON pa.ProductId = a.ProductId AND 
			a.ProductModelTypeId in (SELECT ProductModelTypeId 
									 FROM ProductModelTypes
									 WHERE [Description] not in ('Other','Accessory','System'))