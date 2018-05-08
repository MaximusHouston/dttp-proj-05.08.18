CREATE PROCEDURE [dbo].[spUpdateProductListPriceForSystems]
AS
	UPDATE S
	SET S.ListPrice = SYSTEMTOTALS.SystemPrice
	--SELECT * 
	FROM Products S
	INNER JOIN	(
					-- Calculate list price of the each system, sum summing all accessory products
					SELECT  sc.ProductId as SystemProductId, SUM(sc.ComponentListPrice * sc.ComponentQuantity) AS SystemPrice
					FROM  [VwProductSystemComponents] sc
					GROUP BY sc.ProductId 
					) SYSTEMTOTALS on SYSTEMTOTALS.SystemProductId  = S.ProductId 
	WHERE S.ListPrice != SYSTEMTOTALS.SystemPrice


