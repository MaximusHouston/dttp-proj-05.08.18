CREATE VIEW [dbo].[VwProductDocuments]
AS 
SELECT P.ProductId,Pl.[Rank],D.[DocumentId],D.[DocumentTypeId],DocumentTypeDescription = T.[Description],D.[FileName],D.[CreatedOn],D.[ModifiedOn],D.[Timestamp] 
FROM [dbo].[Products] P
JOIN [dbo].[DocumentProductLinks] PL ON PL.[ProductId] = P.[ProductId]
JOIN [dbo].[Documents] D ON PL.[DocumentId] = D.[DocumentId] 
JOIN [dbo].[DocumentTypes] T ON T.[DocumentTypeId] = D.[DocumentTypeId] 
