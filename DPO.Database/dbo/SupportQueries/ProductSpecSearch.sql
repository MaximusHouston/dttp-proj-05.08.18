/*
SELECT  *
FROM  Products p
INNER JOIN [dbo].[DocumentProductLinks] pa ON p.ProductId = pa.ProductId
INNER JOIN [dbo].[Documents] d ON d.DocumentId = pa.DocumentId
WHERE p.ProductNumber in('REYQ312PBYD','REYQ312PBTJ')
*/