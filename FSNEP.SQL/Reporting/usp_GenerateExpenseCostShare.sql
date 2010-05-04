IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'usp_GenerateExpenseCostShare')
	BEGIN
		DROP  Procedure  usp_GenerateExpenseCostShare
	END

GO

CREATE Procedure usp_GenerateExpenseCostShare
	(
		@year int,
		@projectid varchar(20)
	)
AS

IF @projectID IS NULL SET @projectID = '%'

declare @q1year int
SET @q1year = @year - 1

select *
from (
	select ExpenseTypes.Name
		-- Quarter 1
	  , sum(IsNull(October.Amount, 0)) OctoberAmount, sum(IsNull(October.AmountIndr, 0)) OctoberAmountIndr
	  , sum(IsNull(November.Amount, 0)) NovemberAmount, sum(IsNull(November.AmountIndr, 0)) NovemberAmountIndr
	  , sum(IsNull(December.Amount, 0)) DecemberAmount, sum(IsNull(December.AmountIndr, 0)) DecemberAmountIndr
	  
	  , sum(IsNull(October.Amount, 0)) + sum(IsNull(November.Amount, 0)) + sum(IsNull(December.Amount, 0)) as Q1Amount
	  , sum(IsNull(October.AmountIndr, 0)) + sum(IsNull(November.AmountIndr, 0)) + sum(IsNull(December.AmountIndr, 0)) as Q1AmountIndr
		
		-- Q2
	  , sum(IsNull(January.Amount, 0)) JanuaryAmount, sum(IsNull(January.AmountIndr, 0)) JanuaryAmountIndr
	  , sum(IsNull(February.Amount, 0)) FebruaryAmount, sum(IsNull(February.AmountIndr, 0)) FebruaryAmountIndr
	  , sum(IsNull(March.Amount, 0)) MarchAmount, sum(IsNull(March.AmountIndr, 0)) MarchAmountIndr
	  
	  , sum(IsNull(January.Amount, 0)) + sum(IsNull(February.Amount, 0)) + sum(IsNull(March.Amount, 0)) as Q2Amount
	  , sum(IsNull(January.AmountIndr, 0)) + sum(IsNull(February.AmountIndr, 0)) + sum(IsNull(March.AmountIndr, 0)) as Q2AmountIndr
	  	  
		-- Q3
	  , sum(IsNull(April.Amount, 0)) AprilAmount, sum(IsNull(April.AmountIndr, 0)) AprilAmountIndr
	  , sum(IsNull(May.Amount, 0)) MayAmount, sum(IsNull(May.AmountIndr, 0)) MayAmountIndr
	  , sum(IsNull(June.Amount, 0)) JuneAmount, sum(IsNull(June.AmountIndr, 0)) JuneAmountIndr
	  
	  , sum(IsNull(April.Amount, 0)) + sum(IsNull(May.Amount, 0)) + sum(IsNull(June.Amount, 0)) as Q3Amount
	  , sum(IsNull(April.AmountIndr, 0)) + sum(IsNull(May.AmountIndr, 0)) + sum(IsNull(June.AmountIndr, 0)) as Q3AmountIndr
	  
		-- Q4
	  , sum(IsNull(July.Amount, 0)) JulyAmount, sum(IsNull(July.AmountIndr, 0)) JulyAmountIndr
	  , sum(IsNull(August.Amount, 0)) AugustAmount, sum(IsNull(August.AmountIndr, 0)) AugustAmountIndr
	  , sum(IsNull(September.Amount, 0)) SeptemberAmount, sum(IsNull(September.AmountIndr, 0)) SeptemberAmountIndr
	  
	  , sum(IsNull(July.Amount, 0)) + sum(IsNull(August.Amount, 0)) + sum(IsNull(September.Amount, 0)) as Q4Amount
	  , sum(IsNull(July.AmountIndr, 0)) + sum(IsNull(August.AmountIndr, 0)) + sum(IsNull(September.AmountIndr, 0)) as Q4AmountIndr
	  
	  from ExpenseTypes
		left outer join fsnep.dbo.udf_sumofexpensesbymonth(@q1year, 10, @projectid) October on ExpenseTypes.ID = October.ExpenseTypeID
		left outer join fsnep.dbo.udf_sumofexpensesbymonth(@q1year, 11, @projectid) November on ExpenseTypes.ID = November.ExpenseTypeID
		left outer join fsnep.dbo.udf_sumofexpensesbymonth(@q1year, 12, @projectid) December on ExpenseTypes.ID = December.ExpenseTypeID
		left outer join fsnep.dbo.udf_sumofexpensesbymonth(@year, 1, @projectid) January on ExpenseTypes.ID = January.ExpenseTypeID
		left outer join fsnep.dbo.udf_sumofexpensesbymonth(@year, 2, @projectid) February on ExpenseTypes.ID = February.ExpenseTypeID
		left outer join fsnep.dbo.udf_sumofexpensesbymonth(@year, 3, @projectid) March on ExpenseTypes.ID = March.ExpenseTypeID
		left outer join fsnep.dbo.udf_sumofexpensesbymonth(@year, 4, @projectid) April on ExpenseTypes.ID = April.ExpenseTypeID
		left outer join fsnep.dbo.udf_sumofexpensesbymonth(@year, 5, @projectid) May on ExpenseTypes.ID = May.ExpenseTypeID
		left outer join fsnep.dbo.udf_sumofexpensesbymonth(@year, 6, @projectid) June on ExpenseTypes.ID = June.ExpenseTypeID
		left outer join fsnep.dbo.udf_sumofexpensesbymonth(@year, 7, @projectid) July on ExpenseTypes.ID = July.ExpenseTypeID
		left outer join fsnep.dbo.udf_sumofexpensesbymonth(@year, 8, @projectid) August on ExpenseTypes.ID = August.ExpenseTypeID
		left outer join fsnep.dbo.udf_sumofexpensesbymonth(@year, 9, @projectid) September on ExpenseTypes.ID = September.ExpenseTypeID
	group by ExpenseTypes.Name
	) totals
	order by Name

GO

GRANT EXEC ON usp_GenerateExpenseCostShare TO PUBLIC

GO

