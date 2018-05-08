CREATE VIEW [dbo].[VwProductNotes]
AS 
SELECT P.ProductId,PN.[ProductNoteTypeId],ProdcutNoteTypeDescription = T.[Description], PN.[Description],PN.Rank, PN.[ModifiedOn], PN.[Timestamp] 
FROM [dbo].[Products] P
JOIN [dbo].[ProductNotes] PN ON PN.[ProductId] = P.[ProductId]
JOIN [dbo].[ProductNoteTypes] T ON T.[ProductNoteTypeId] = PN.[ProductNoteTypeId] 
