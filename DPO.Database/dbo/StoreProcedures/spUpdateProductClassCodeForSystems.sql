CREATE PROCEDURE [dbo].[spUpdateProductClassCodeForSystems]
AS

    /* Net price calculations and multipliers are not applied for assembled 
    * products when there is no product class code. 
    * For now we are to move the first product class code from a sub component to the parent if none exists.
    * Some indoor products are made of a number of other indoor products, these are not in the ERP system so they don't have a product family. 
    * So for now we use the sub component's product family. This is needed for multipliers and net price calculations.
    * In the future we might need to revisit this as this assumes all sub products share the same multiplier, 
    * which for now is the case.*/

	UPDATE P
	SET ProductClassCode = S.FirstComponentClassCode
   --select * 
	FROM Products P
	INNER JOIN (
				SELECT  ProductId, FirstComponentClassCode = (
									SELECT top 1 p.ProductClassCode
									FROM  [VwProductSystemComponents] sc
									INNER JOIN Products P ON P.ProductId = sc.ComponentProductId
									where sc.ProductId = s.ProductId )
				FROM Products S
	) S ON p.ProductId = S.ProductId
	where ProductClassCode != FirstComponentClassCode and FirstComponentClassCode is not null