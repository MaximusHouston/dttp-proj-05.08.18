CREATE PROCEDURE [dbo].[spUpdateRecalculationRequiredForQuotes]
AS

	update q set [RecalculationRequired] = 0
	from [dbo].[Quotes] q
	where [RecalculationRequired] = 1

	update q set [RecalculationRequired] = 1
	from Quotes q
	join Projects pj on q.ProjectId = pj.ProjectId
	join QuoteItems qi on q.QuoteId = qi.QuoteId
	join Products p on qi.ProductId = p.ProductId
	left join AccountMultipliers m on qi.AccountMultiplierId = m.AccountMultiplierId 
	where pj.ProjectStatusTypeId = 1
		and ( qi.ListPrice != p.ListPrice or (q.IsCommissionScheme = 0 and m.Multiplier != qi.Multiplier) )
