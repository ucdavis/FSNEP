 IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[udf_SumOfHoursByMonth]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
	DROP FUNCTION [dbo].[udf_SumOfHoursByMonth]
GO

CREATE FUNCTION [dbo].[udf_SumOfHoursByMonth]
(	
	@year int,
	@month int,		-- the number of the month
	@projectid int,	-- the project id requested
	@fundtype varchar(50) -- the fundType to return
)
RETURNS TABLE 
AS

RETURN 
(
	select rec.userid, rec.month, rec.year
		, sum(TotalHours.hours) hours
		, sum(round(TotalHours.hours / him.hoursinmonth, 3)) FTE
		, sum(round(ts.salary * totalhours.hours, 2)) Pay
		, sum(round(ts.salary * totalhours.hours * benefitrate, 2)) Benefits
		, sum(round(ts.salary * totalhours.hours, 2) + round(ts.salary * totalhours.hours * benefitrate, 2)) Total
		, sum((round(ts.salary * totalhours.hours, 2) + round(ts.salary * totalhours.hours * benefitrate, 2)) * fa.IndirectCost) TotalIndr
	from TimeRecords ts
		inner join Records rec on ts.ID = rec.ID
		inner join (
			select ent.RecordID, sum(tse.Hours) Hours, ent.fundtypeid, ent.projectid, ent.financeAccountID
			from TimeRecordEntries tse
				inner join Entries ent on tse.ID = ent.ID
				inner join fundtypes ft on ft.id = ent.fundtypeid
			where 
				ft.name = @fundtype
				and ent.ProjectID = @projectid
			group by ent.RecordID, ent.fundtypeid, ent.projectid, ent.financeAccountID
		) TotalHours on TotalHours.RecordID = rec.id
		left outer join hoursinmonths him on cast(him.month as varchar(2)) + cast(him.year as varchar(4)) = cast(rec.month as varchar(2)) + cast(rec.year as varchar(4))
		inner join users on users.userid = rec.userid
		inner join financeaccounts fa on fa.id = TotalHours.FinanceAccountID
	where rec.year = @year
		and rec.month = @month
		and statusid in ( select id from status where [Name] = 'Approved' )
	group by rec.userid, rec.month, rec.year, him.hoursinmonth
)
GO

GRANT SELECT ON [udf_SumOfHoursByMonth] TO PUBLIC

GO
