
CREATE FUNCTION [dbo].[udf_SumOfExpensesByMonth]
(	
	@year int,
	@month int,		-- the number of the month
	@projectid varchar(20)	-- the project id requested
)
RETURNS TABLE 
AS

RETURN 
(
	select TotalAmount.expenseTypeID
		, rec.month
		, rec.year
		, sum(TotalAmount.Amount) amount
		, CASE et.Name
				WHEN 'Supplies' THEN SUM(TotalAmount.Amount * fa.IndirectCost)
				WHEN 'Teacher Time $' THEN SUM(TotalAmount.Amount * fa.IndirectCost)
				WHEN 'Travel' THEN SUM(TotalAmount.Amount * fa.IndirectCost)
				ELSE 0.0
			END	as AmountIndr
	from CostShareRecords cs
			inner join Records rec on cs.ID = rec.ID
			inner join (
				select ent.RecordID, sum(cse.ExpenseAmount) Amount, ent.ProjectID, cse.ExpenseTypeID, ent.FinanceAccountID
				from CostShareRecordEntries cse
					inner join Entries ent on cse.ID = ent.ID
				where ent.ProjectID LIKE @projectid
				group by ent.RecordID, cse.ExpenseTypeID, ent.ProjectID, ent.FinanceAccountID
			) TotalAmount on TotalAmount.RecordID= rec.ID
			inner join Users on Users.UserId = rec.UserId
			inner join FinanceAccounts fa on fa.ID = TotalAmount.FinanceAccountID
			inner join ExpenseTypes et on et.ID = TotalAmount.ExpenseTypeID
		where rec.Year = @year AND rec.Month = @month
				AND rec.StatusID in 
					( select ID from [Status] where Name = 'Approved' )
		group by ExpenseTypeID, et.Name, rec.Month, rec.Year
)