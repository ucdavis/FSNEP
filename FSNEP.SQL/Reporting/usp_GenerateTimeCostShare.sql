IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'usp_GenerateTimeCostShare')
	BEGIN
		DROP  Procedure  usp_GenerateTimeCostShare
	END

GO

CREATE Procedure usp_GenerateTimeCostShare
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
	select users.firstname, users.lastname, 'State Share' FundType
		-- Quarter 1
	  , sum(IsNull(October.hours, 0)) OctoberHours, sum(IsNull(October.fte, 0)) OctoberFTE, sum(IsNull(october.pay, 0)) OctoberPay, sum(IsNull(october.benefits, 0)) OctoberBenefits, sum(IsNull(october.total, 0)) OctoberTotal, sum(IsNull(october.TotalIndr, 0)) octoberTotalIndr
	  , sum(IsNull(November.hours, 0)) NovemberHours, sum(IsNull(November.fte, 0)) NovemberFTE, sum(IsNull(november.pay, 0)) NovemberPay, sum(IsNull(november.benefits, 0)) NovemberBenefits, sum(IsNull(november.total, 0)) NovemberTotal, sum(IsNull(november.TotalIndr, 0)) novemberTotalIndr
	  , sum(IsNull(December.hours, 0)) DecemberHours, sum(IsNull(December.fte, 0)) DecemberFTE, sum(IsNull(December.pay, 0)) DecemberPay, sum(IsNull(December.benefits, 0)) DecemberBenefits, sum(IsNull(December.total, 0)) DecemberTotal, sum(IsNull(December.TotalIndr, 0)) DecemberTotalIndr
      
	  , sum(IsNull(October.hours, 0)) + sum(IsNull(November.hours, 0)) + sum(IsNull(December.hours, 0)) as Q1Hours
	  , (sum(IsNull(October.hours, 0)) + sum(IsNull(November.hours, 0)) + sum(IsNull(December.hours, 0))) / ( dbo.udf_QuarterTotalHours(@q1year, 'Q1') ) as Q1fte
	  , sum(IsNull(October.pay, 0)) + sum(IsNull(November.pay, 0)) + sum(IsNull(December.pay, 0)) as Q1pay, sum(IsNull(October.benefits, 0)) + sum(IsNull(November.benefits, 0)) + sum(IsNull(December.benefits, 0)) as Q1benefits
	  , sum(IsNull(October.total, 0)) + sum(IsNull(November.total, 0)) + sum(IsNull(December.total, 0)) as Q1total, sum(IsNull(October.TotalIndr, 0)) + sum(IsNull(November.TotalIndr, 0)) + sum(IsNull(December.TotalIndr, 0)) as Q1TotalIndr

		-- Quarter 2
	  , sum(IsNull(January.hours, 0)) JanuaryHours, sum(IsNull(January.fte, 0)) JanuaryFTE, sum(IsNull(January.pay, 0)) JanuaryPay, sum(IsNull(January.benefits, 0)) JanuaryBenefits, sum(IsNull(January.total, 0)) JanuaryTotal, sum(IsNull(January.TotalIndr, 0)) JanuaryTotalIndr
	  , sum(IsNull(February.hours, 0)) FebruaryHours, sum(IsNull(February.fte, 0)) FebruaryFTE, sum(IsNull(February.pay, 0)) FebruaryPay, sum(IsNull(February.benefits, 0)) FebruaryBenefits, sum(IsNull(February.total, 0)) FebruaryTotal, sum(IsNull(February.TotalIndr, 0)) FebruaryTotalIndr
	  , sum(IsNull(March.hours, 0)) MarchHours, sum(IsNull(March.fte, 0)) MarchFTE, sum(IsNull(March.pay, 0)) MarchPay, sum(IsNull(March.benefits, 0)) MarchBenefits, sum(IsNull(March.total, 0)) MarchTotal, sum(IsNull(March.TotalIndr, 0)) MarchTotalIndr
      
	  , sum(IsNull(January.hours, 0)) + sum(IsNull(February.hours, 0)) + sum(IsNull(March.hours, 0)) as Q2Hours
	  , (sum(IsNull(January.hours, 0)) + sum(IsNull(February.hours, 0)) + sum(IsNull(March.hours, 0))) / ( dbo.udf_QuarterTotalHours(@year, 'Q2') ) as Q2fte
	  , sum(IsNull(January.pay, 0)) + sum(IsNull(February.pay, 0)) + sum(IsNull(March.pay, 0)) as Q2pay, sum(IsNull(January.benefits, 0)) + sum(IsNull(February.benefits, 0)) + sum(IsNull(March.benefits, 0)) as Q2benefits
	  , sum(IsNull(January.total, 0)) + sum(IsNull(February.total, 0)) + sum(IsNull(March.total, 0)) as Q2total, sum(IsNull(January.TotalIndr, 0)) + sum(IsNull(February.TotalIndr, 0)) + sum(IsNull(March.TotalIndr, 0)) as Q2TotalIndr

		-- Quarter 3
	  , sum(IsNull(April.hours, 0)) AprilHours, sum(IsNull(April.fte, 0)) AprilFTE, sum(IsNull(April.pay, 0)) AprilPay, sum(IsNull(April.benefits, 0)) AprilBenefits, sum(IsNull(April.total, 0)) AprilTotal, sum(IsNull(April.TotalIndr, 0)) AprilTotalIndr
	  , sum(IsNull(May.hours, 0)) MayHours, sum(IsNull(May.fte, 0)) MayFTE, sum(IsNull(May.pay, 0)) MayPay, sum(IsNull(May.benefits, 0)) MayBenefits, sum(IsNull(May.total, 0)) MayTotal, sum(IsNull(May.TotalIndr, 0)) MayTotalIndr
	  , sum(IsNull(June.hours, 0)) JuneHours, sum(IsNull(June.fte, 0)) JuneFTE, sum(IsNull(June.pay, 0)) JunePay, sum(IsNull(June.benefits, 0)) JuneBenefits, sum(IsNull(June.total, 0)) JuneTotal, sum(IsNull(June.TotalIndr, 0)) JuneTotalIndr
      
	  , sum(IsNull(April.hours, 0)) + sum(IsNull(May.hours, 0)) + sum(IsNull(June.hours, 0)) as Q3Hours
	  , (sum(IsNull(April.hours, 0)) + sum(IsNull(May.hours, 0)) + sum(IsNull(June.hours, 0))) / ( dbo.udf_QuarterTotalHours(@year, 'Q3') ) as Q3fte
	  , sum(IsNull(April.pay, 0)) + sum(IsNull(May.pay, 0)) + sum(IsNull(June.pay, 0)) as Q3pay, sum(IsNull(April.benefits, 0)) + sum(IsNull(May.benefits, 0)) + sum(IsNull(June.benefits, 0)) as Q3benefits
	  , sum(IsNull(April.total, 0)) + sum(IsNull(May.total, 0)) + sum(IsNull(June.total, 0)) as Q3total, sum(IsNull(April.TotalIndr, 0)) + sum(IsNull(May.TotalIndr, 0)) + sum(IsNull(June.TotalIndr, 0)) as Q3TotalIndr
		
		-- Quarter 4
	  , sum(IsNull(July.hours, 0)) JulyHours, sum(IsNull(July.fte, 0)) JulyFTE, sum(IsNull(July.pay, 0)) JulyPay, sum(IsNull(July.benefits, 0)) JulyBenefits, sum(IsNull(July.total, 0)) JulyTotal, sum(IsNull(July.TotalIndr, 0)) JulyTotalIndr
	  , sum(IsNull(August.hours, 0)) AugustHours, sum(IsNull(August.fte, 0)) AugustFTE, sum(IsNull(August.pay, 0)) AugustPay, sum(IsNull(August.benefits, 0)) AugustBenefits, sum(IsNull(August.total, 0)) AugustTotal, sum(IsNull(August.TotalIndr, 0)) AugustTotalIndr
	  , sum(IsNull(September.hours, 0)) SeptemberHours, sum(IsNull(September.fte, 0)) SeptemberFTE, sum(IsNull(September.pay, 0)) SeptemberPay, sum(IsNull(September.benefits, 0)) SeptemberBenefits, sum(IsNull(September.total, 0)) SeptemberTotal, sum(IsNull(September.TotalIndr, 0)) SeptemberTotalIndr
      
	  , sum(IsNull(July.hours, 0)) + sum(IsNull(August.hours, 0)) + sum(IsNull(September.hours, 0)) as Q4Hours
	  , (sum(IsNull(July.hours, 0)) + sum(IsNull(August.hours, 0)) + sum(IsNull(September.hours, 0))) / ( dbo.udf_QuarterTotalHours(@year, 'Q4') ) as Q4fte
	  , sum(IsNull(July.pay, 0)) + sum(IsNull(August.pay, 0)) + sum(IsNull(September.pay, 0)) as Q4pay, sum(IsNull(July.benefits, 0)) + sum(IsNull(August.benefits, 0)) + sum(IsNull(September.benefits, 0)) as Q4benefits
	  , sum(IsNull(July.total, 0)) + sum(IsNull(August.total, 0)) + sum(IsNull(September.total, 0)) as Q4total, sum(IsNull(July.TotalIndr, 0)) + sum(IsNull(August.TotalIndr, 0)) + sum(IsNull(September.TotalIndr, 0)) as Q4TotalIndr
	  
	  , [dbo].[udf_QuarterTotalHours] (@year, 'FN') as FinalHours
	  
	from users
		left outer join udf_sumofhoursbymonth(@q1year, 10, @projectid, 'State Share') October on users.userid = October.userid
		left outer join udf_sumofhoursbymonth(@q1year, 11, @projectid, 'State Share') November on users.userid = November.userid
		left outer join udf_sumofhoursbymonth(@q1year, 12, @projectid, 'State Share') December on users.userid = December.userid
		left outer join udf_sumofhoursbymonth(@year, 1, @projectid, 'State Share') January on users.userid = January.userid
		left outer join udf_sumofhoursbymonth(@year, 2, @projectid, 'State Share') February on users.userid = February.userid
		left outer join udf_sumofhoursbymonth(@year, 3, @projectid, 'State Share') March on users.userid = March.userid
		left outer join udf_sumofhoursbymonth(@year, 4, @projectid, 'State Share') April on users.userid = April.userid
		left outer join udf_sumofhoursbymonth(@year, 5, @projectid, 'State Share') May on users.userid = May.userid
		left outer join udf_sumofhoursbymonth(@year, 6, @projectid, 'State Share') June on users.userid = June.userid
		left outer join udf_sumofhoursbymonth(@year, 7, @projectid, 'State Share') July on users.userid = July.userid
		left outer join udf_sumofhoursbymonth(@year, 8, @projectid, 'State Share') August on users.userid = August.userid
		left outer join udf_sumofhoursbymonth(@year, 9, @projectid, 'State Share') September on users.userid = September.userid
	--where users.isactive = 1
	group by users.firstname, users.lastname
	) totals
where (
	octoberhours <> 0
	or novemberhours <> 0
	or decemberhours <> 0
	or januaryhours <> 0
	or februaryhours <> 0
	or marchhours <> 0
	or aprilhours <> 0
	or mayhours <> 0
	or junehours <> 0
	or julyhours <> 0
	or augusthours <> 0
	or septemberhours <> 0
)
union
select *
from (
	select users.firstname, users.lastname, 'State Share 3rd Party' FundType
		-- Quarter 1
	  , sum(IsNull(October.hours, 0)) OctoberHours, sum(IsNull(October.fte, 0)) OctoberFTE, sum(IsNull(october.pay, 0)) OctoberPay, sum(IsNull(october.benefits, 0)) OctoberBenefits, sum(IsNull(october.total, 0)) OctoberTotal, sum(IsNull(october.TotalIndr, 0)) octoberTotalIndr
	  , sum(IsNull(November.hours, 0)) NovemberHours, sum(IsNull(November.fte, 0)) NovemberFTE, sum(IsNull(november.pay, 0)) NovemberPay, sum(IsNull(november.benefits, 0)) NovemberBenefits, sum(IsNull(november.total, 0)) NovemberTotal, sum(IsNull(november.TotalIndr, 0)) novemberTotalIndr
	  , sum(IsNull(December.hours, 0)) DecemberHours, sum(IsNull(December.fte, 0)) DecemberFTE, sum(IsNull(December.pay, 0)) DecemberPay, sum(IsNull(December.benefits, 0)) DecemberBenefits, sum(IsNull(December.total, 0)) DecemberTotal, sum(IsNull(December.TotalIndr, 0)) DecemberTotalIndr
      
	  , sum(IsNull(October.hours, 0)) + sum(IsNull(November.hours, 0)) + sum(IsNull(December.hours, 0)) as Q1Hours
	  , (sum(IsNull(October.hours, 0)) + sum(IsNull(November.hours, 0)) + sum(IsNull(December.hours, 0))) / ( dbo.udf_QuarterTotalHours(@q1year, 'Q1') ) as Q1fte
	  , sum(IsNull(October.pay, 0)) + sum(IsNull(November.pay, 0)) + sum(IsNull(December.pay, 0)) as Q1pay, sum(IsNull(October.benefits, 0)) + sum(IsNull(November.benefits, 0)) + sum(IsNull(December.benefits, 0)) as Q1benefits
	  , sum(IsNull(October.total, 0)) + sum(IsNull(November.total, 0)) + sum(IsNull(December.total, 0)) as Q1total, sum(IsNull(October.TotalIndr, 0)) + sum(IsNull(November.TotalIndr, 0)) + sum(IsNull(December.TotalIndr, 0)) as Q1TotalIndr

		-- Quarter 2
	  , sum(IsNull(January.hours, 0)) JanuaryHours, sum(IsNull(January.fte, 0)) JanuaryFTE, sum(IsNull(January.pay, 0)) JanuaryPay, sum(IsNull(January.benefits, 0)) JanuaryBenefits, sum(IsNull(January.total, 0)) JanuaryTotal, sum(IsNull(January.TotalIndr, 0)) JanuaryTotalIndr
	  , sum(IsNull(February.hours, 0)) FebruaryHours, sum(IsNull(February.fte, 0)) FebruaryFTE, sum(IsNull(February.pay, 0)) FebruaryPay, sum(IsNull(February.benefits, 0)) FebruaryBenefits, sum(IsNull(February.total, 0)) FebruaryTotal, sum(IsNull(February.TotalIndr, 0)) FebruaryTotalIndr
	  , sum(IsNull(March.hours, 0)) MarchHours, sum(IsNull(March.fte, 0)) MarchFTE, sum(IsNull(March.pay, 0)) MarchPay, sum(IsNull(March.benefits, 0)) MarchBenefits, sum(IsNull(March.total, 0)) MarchTotal, sum(IsNull(March.TotalIndr, 0)) MarchTotalIndr
      
	  , sum(IsNull(January.hours, 0)) + sum(IsNull(February.hours, 0)) + sum(IsNull(March.hours, 0)) as Q2Hours
	  , (sum(IsNull(January.hours, 0)) + sum(IsNull(February.hours, 0)) + sum(IsNull(March.hours, 0))) / ( dbo.udf_QuarterTotalHours(@year, 'Q2') ) as Q2fte
	  , sum(IsNull(January.pay, 0)) + sum(IsNull(February.pay, 0)) + sum(IsNull(March.pay, 0)) as Q2pay, sum(IsNull(January.benefits, 0)) + sum(IsNull(February.benefits, 0)) + sum(IsNull(March.benefits, 0)) as Q2benefits
	  , sum(IsNull(January.total, 0)) + sum(IsNull(February.total, 0)) + sum(IsNull(March.total, 0)) as Q2total, sum(IsNull(January.TotalIndr, 0)) + sum(IsNull(February.TotalIndr, 0)) + sum(IsNull(March.TotalIndr, 0)) as Q2TotalIndr

		-- Quarter 3
	  , sum(IsNull(April.hours, 0)) AprilHours, sum(IsNull(April.fte, 0)) AprilFTE, sum(IsNull(April.pay, 0)) AprilPay, sum(IsNull(April.benefits, 0)) AprilBenefits, sum(IsNull(April.total, 0)) AprilTotal, sum(IsNull(April.TotalIndr, 0)) AprilTotalIndr
	  , sum(IsNull(May.hours, 0)) MayHours, sum(IsNull(May.fte, 0)) MayFTE, sum(IsNull(May.pay, 0)) MayPay, sum(IsNull(May.benefits, 0)) MayBenefits, sum(IsNull(May.total, 0)) MayTotal, sum(IsNull(May.TotalIndr, 0)) MayTotalIndr
	  , sum(IsNull(June.hours, 0)) JuneHours, sum(IsNull(June.fte, 0)) JuneFTE, sum(IsNull(June.pay, 0)) JunePay, sum(IsNull(June.benefits, 0)) JuneBenefits, sum(IsNull(June.total, 0)) JuneTotal, sum(IsNull(June.TotalIndr, 0)) JuneTotalIndr
      
	  , sum(IsNull(April.hours, 0)) + sum(IsNull(May.hours, 0)) + sum(IsNull(June.hours, 0)) as Q3Hours
	  , (sum(IsNull(April.hours, 0)) + sum(IsNull(May.hours, 0)) + sum(IsNull(June.hours, 0))) / ( dbo.udf_QuarterTotalHours(@year, 'Q3') ) as Q3fte
	  , sum(IsNull(April.pay, 0)) + sum(IsNull(May.pay, 0)) + sum(IsNull(June.pay, 0)) as Q3pay, sum(IsNull(April.benefits, 0)) + sum(IsNull(May.benefits, 0)) + sum(IsNull(June.benefits, 0)) as Q3benefits
	  , sum(IsNull(April.total, 0)) + sum(IsNull(May.total, 0)) + sum(IsNull(June.total, 0)) as Q3total, sum(IsNull(April.TotalIndr, 0)) + sum(IsNull(May.TotalIndr, 0)) + sum(IsNull(June.TotalIndr, 0)) as Q3TotalIndr
		
		-- Quarter 4
	  , sum(IsNull(July.hours, 0)) JulyHours, sum(IsNull(July.fte, 0)) JulyFTE, sum(IsNull(July.pay, 0)) JulyPay, sum(IsNull(July.benefits, 0)) JulyBenefits, sum(IsNull(July.total, 0)) JulyTotal, sum(IsNull(July.TotalIndr, 0)) JulyTotalIndr
	  , sum(IsNull(August.hours, 0)) AugustHours, sum(IsNull(August.fte, 0)) AugustFTE, sum(IsNull(August.pay, 0)) AugustPay, sum(IsNull(August.benefits, 0)) AugustBenefits, sum(IsNull(August.total, 0)) AugustTotal, sum(IsNull(August.TotalIndr, 0)) AugustTotalIndr
	  , sum(IsNull(September.hours, 0)) SeptemberHours, sum(IsNull(September.fte, 0)) SeptemberFTE, sum(IsNull(September.pay, 0)) SeptemberPay, sum(IsNull(September.benefits, 0)) SeptemberBenefits, sum(IsNull(September.total, 0)) SeptemberTotal, sum(IsNull(September.TotalIndr, 0)) SeptemberTotalIndr
      
	  , sum(IsNull(July.hours, 0)) + sum(IsNull(August.hours, 0)) + sum(IsNull(September.hours, 0)) as Q4Hours
	  , (sum(IsNull(July.hours, 0)) + sum(IsNull(August.hours, 0)) + sum(IsNull(September.hours, 0))) / ( dbo.udf_QuarterTotalHours(@year, 'Q4') ) as Q4fte
	  , sum(IsNull(July.pay, 0)) + sum(IsNull(August.pay, 0)) + sum(IsNull(September.pay, 0)) as Q4pay, sum(IsNull(July.benefits, 0)) + sum(IsNull(August.benefits, 0)) + sum(IsNull(September.benefits, 0)) as Q4benefits
	  , sum(IsNull(July.total, 0)) + sum(IsNull(August.total, 0)) + sum(IsNull(September.total, 0)) as Q4total, sum(IsNull(July.TotalIndr, 0)) + sum(IsNull(August.TotalIndr, 0)) + sum(IsNull(September.TotalIndr, 0)) as Q4TotalIndr
	  
	  , [dbo].[udf_QuarterTotalHours] (@year, 'FN') as FinalHours
	  
	from users
		left outer join udf_sumofhoursbymonth(@q1year, 10, @projectid, 'State Share 3rd Party') October on users.userid = October.userid
		left outer join udf_sumofhoursbymonth(@q1year, 11, @projectid, 'State Share 3rd Party') November on users.userid = November.userid
		left outer join udf_sumofhoursbymonth(@q1year, 12, @projectid, 'State Share 3rd Party') December on users.userid = December.userid
		left outer join udf_sumofhoursbymonth(@year, 1, @projectid, 'State Share 3rd Party') January on users.userid = January.userid
		left outer join udf_sumofhoursbymonth(@year, 2, @projectid, 'State Share 3rd Party') February on users.userid = February.userid
		left outer join udf_sumofhoursbymonth(@year, 3, @projectid, 'State Share 3rd Party') March on users.userid = March.userid
		left outer join udf_sumofhoursbymonth(@year, 4, @projectid, 'State Share 3rd Party') April on users.userid = April.userid
		left outer join udf_sumofhoursbymonth(@year, 5, @projectid, 'State Share 3rd Party') May on users.userid = May.userid
		left outer join udf_sumofhoursbymonth(@year, 6, @projectid, 'State Share 3rd Party') June on users.userid = June.userid
		left outer join udf_sumofhoursbymonth(@year, 7, @projectid, 'State Share 3rd Party') July on users.userid = July.userid
		left outer join udf_sumofhoursbymonth(@year, 8, @projectid, 'State Share 3rd Party') August on users.userid = August.userid
		left outer join udf_sumofhoursbymonth(@year, 9, @projectid, 'State Share 3rd Party') September on users.userid = September.userid
	--where users.isactive = 1
	group by users.firstname, users.lastname
	) totals
where (
	octoberhours <> 0
	or novemberhours <> 0
	or decemberhours <> 0
	or januaryhours <> 0
	or februaryhours <> 0
	or marchhours <> 0
	or aprilhours <> 0
	or mayhours <> 0
	or junehours <> 0
	or julyhours <> 0
	or augusthours <> 0
	or septemberhours <> 0
)
union
select *
from (
	select users.firstname, users.lastname, 'State Share Volunteer' FundType
		-- Quarter 1
	  , sum(IsNull(October.hours, 0)) OctoberHours, sum(IsNull(October.fte, 0)) OctoberFTE, sum(IsNull(october.pay, 0)) OctoberPay, sum(IsNull(october.benefits, 0)) OctoberBenefits, sum(IsNull(october.total, 0)) OctoberTotal, sum(IsNull(october.TotalIndr, 0)) octoberTotalIndr
	  , sum(IsNull(November.hours, 0)) NovemberHours, sum(IsNull(November.fte, 0)) NovemberFTE, sum(IsNull(november.pay, 0)) NovemberPay, sum(IsNull(november.benefits, 0)) NovemberBenefits, sum(IsNull(november.total, 0)) NovemberTotal, sum(IsNull(november.TotalIndr, 0)) novemberTotalIndr
	  , sum(IsNull(December.hours, 0)) DecemberHours, sum(IsNull(December.fte, 0)) DecemberFTE, sum(IsNull(December.pay, 0)) DecemberPay, sum(IsNull(December.benefits, 0)) DecemberBenefits, sum(IsNull(December.total, 0)) DecemberTotal, sum(IsNull(December.TotalIndr, 0)) DecemberTotalIndr
      
	  , sum(IsNull(October.hours, 0)) + sum(IsNull(November.hours, 0)) + sum(IsNull(December.hours, 0)) as Q1Hours
	  , (sum(IsNull(October.hours, 0)) + sum(IsNull(November.hours, 0)) + sum(IsNull(December.hours, 0))) / ( dbo.udf_QuarterTotalHours(@q1year, 'Q1') ) as Q1fte
	  , sum(IsNull(October.pay, 0)) + sum(IsNull(November.pay, 0)) + sum(IsNull(December.pay, 0)) as Q1pay, sum(IsNull(October.benefits, 0)) + sum(IsNull(November.benefits, 0)) + sum(IsNull(December.benefits, 0)) as Q1benefits
	  , sum(IsNull(October.total, 0)) + sum(IsNull(November.total, 0)) + sum(IsNull(December.total, 0)) as Q1total, sum(IsNull(October.TotalIndr, 0)) + sum(IsNull(November.TotalIndr, 0)) + sum(IsNull(December.TotalIndr, 0)) as Q1TotalIndr

		-- Quarter 2
	  , sum(IsNull(January.hours, 0)) JanuaryHours, sum(IsNull(January.fte, 0)) JanuaryFTE, sum(IsNull(January.pay, 0)) JanuaryPay, sum(IsNull(January.benefits, 0)) JanuaryBenefits, sum(IsNull(January.total, 0)) JanuaryTotal, sum(IsNull(January.TotalIndr, 0)) JanuaryTotalIndr
	  , sum(IsNull(February.hours, 0)) FebruaryHours, sum(IsNull(February.fte, 0)) FebruaryFTE, sum(IsNull(February.pay, 0)) FebruaryPay, sum(IsNull(February.benefits, 0)) FebruaryBenefits, sum(IsNull(February.total, 0)) FebruaryTotal, sum(IsNull(February.TotalIndr, 0)) FebruaryTotalIndr
	  , sum(IsNull(March.hours, 0)) MarchHours, sum(IsNull(March.fte, 0)) MarchFTE, sum(IsNull(March.pay, 0)) MarchPay, sum(IsNull(March.benefits, 0)) MarchBenefits, sum(IsNull(March.total, 0)) MarchTotal, sum(IsNull(March.TotalIndr, 0)) MarchTotalIndr
      
	  , sum(IsNull(January.hours, 0)) + sum(IsNull(February.hours, 0)) + sum(IsNull(March.hours, 0)) as Q2Hours
	  , (sum(IsNull(January.hours, 0)) + sum(IsNull(February.hours, 0)) + sum(IsNull(March.hours, 0))) / ( dbo.udf_QuarterTotalHours(@year, 'Q2') ) as Q2fte
	  , sum(IsNull(January.pay, 0)) + sum(IsNull(February.pay, 0)) + sum(IsNull(March.pay, 0)) as Q2pay, sum(IsNull(January.benefits, 0)) + sum(IsNull(February.benefits, 0)) + sum(IsNull(March.benefits, 0)) as Q2benefits
	  , sum(IsNull(January.total, 0)) + sum(IsNull(February.total, 0)) + sum(IsNull(March.total, 0)) as Q2total, sum(IsNull(January.TotalIndr, 0)) + sum(IsNull(February.TotalIndr, 0)) + sum(IsNull(March.TotalIndr, 0)) as Q2TotalIndr

		-- Quarter 3
	  , sum(IsNull(April.hours, 0)) AprilHours, sum(IsNull(April.fte, 0)) AprilFTE, sum(IsNull(April.pay, 0)) AprilPay, sum(IsNull(April.benefits, 0)) AprilBenefits, sum(IsNull(April.total, 0)) AprilTotal, sum(IsNull(April.TotalIndr, 0)) AprilTotalIndr
	  , sum(IsNull(May.hours, 0)) MayHours, sum(IsNull(May.fte, 0)) MayFTE, sum(IsNull(May.pay, 0)) MayPay, sum(IsNull(May.benefits, 0)) MayBenefits, sum(IsNull(May.total, 0)) MayTotal, sum(IsNull(May.TotalIndr, 0)) MayTotalIndr
	  , sum(IsNull(June.hours, 0)) JuneHours, sum(IsNull(June.fte, 0)) JuneFTE, sum(IsNull(June.pay, 0)) JunePay, sum(IsNull(June.benefits, 0)) JuneBenefits, sum(IsNull(June.total, 0)) JuneTotal, sum(IsNull(June.TotalIndr, 0)) JuneTotalIndr
      
	  , sum(IsNull(April.hours, 0)) + sum(IsNull(May.hours, 0)) + sum(IsNull(June.hours, 0)) as Q3Hours
	  , (sum(IsNull(April.hours, 0)) + sum(IsNull(May.hours, 0)) + sum(IsNull(June.hours, 0))) / ( dbo.udf_QuarterTotalHours(@year, 'Q3') ) as Q3fte
	  , sum(IsNull(April.pay, 0)) + sum(IsNull(May.pay, 0)) + sum(IsNull(June.pay, 0)) as Q3pay, sum(IsNull(April.benefits, 0)) + sum(IsNull(May.benefits, 0)) + sum(IsNull(June.benefits, 0)) as Q3benefits
	  , sum(IsNull(April.total, 0)) + sum(IsNull(May.total, 0)) + sum(IsNull(June.total, 0)) as Q3total, sum(IsNull(April.TotalIndr, 0)) + sum(IsNull(May.TotalIndr, 0)) + sum(IsNull(June.TotalIndr, 0)) as Q3TotalIndr
		
		-- Quarter 4
	  , sum(IsNull(July.hours, 0)) JulyHours, sum(IsNull(July.fte, 0)) JulyFTE, sum(IsNull(July.pay, 0)) JulyPay, sum(IsNull(July.benefits, 0)) JulyBenefits, sum(IsNull(July.total, 0)) JulyTotal, sum(IsNull(July.TotalIndr, 0)) JulyTotalIndr
	  , sum(IsNull(August.hours, 0)) AugustHours, sum(IsNull(August.fte, 0)) AugustFTE, sum(IsNull(August.pay, 0)) AugustPay, sum(IsNull(August.benefits, 0)) AugustBenefits, sum(IsNull(August.total, 0)) AugustTotal, sum(IsNull(August.TotalIndr, 0)) AugustTotalIndr
	  , sum(IsNull(September.hours, 0)) SeptemberHours, sum(IsNull(September.fte, 0)) SeptemberFTE, sum(IsNull(September.pay, 0)) SeptemberPay, sum(IsNull(September.benefits, 0)) SeptemberBenefits, sum(IsNull(September.total, 0)) SeptemberTotal, sum(IsNull(September.TotalIndr, 0)) SeptemberTotalIndr
      
	  , sum(IsNull(July.hours, 0)) + sum(IsNull(August.hours, 0)) + sum(IsNull(September.hours, 0)) as Q4Hours
	  , (sum(IsNull(July.hours, 0)) + sum(IsNull(August.hours, 0)) + sum(IsNull(September.hours, 0))) / ( dbo.udf_QuarterTotalHours(@year, 'Q4') ) as Q4fte
	  , sum(IsNull(July.pay, 0)) + sum(IsNull(August.pay, 0)) + sum(IsNull(September.pay, 0)) as Q4pay, sum(IsNull(July.benefits, 0)) + sum(IsNull(August.benefits, 0)) + sum(IsNull(September.benefits, 0)) as Q4benefits
	  , sum(IsNull(July.total, 0)) + sum(IsNull(August.total, 0)) + sum(IsNull(September.total, 0)) as Q4total, sum(IsNull(July.TotalIndr, 0)) + sum(IsNull(August.TotalIndr, 0)) + sum(IsNull(September.TotalIndr, 0)) as Q4TotalIndr
	  
	  , [dbo].[udf_QuarterTotalHours] (@year, 'FN') as FinalHours
	  
	from users
		left outer join udf_sumofhoursbymonth(@q1year, 10, @projectid, 'State Share Volunteer') October on users.userid = October.userid
		left outer join udf_sumofhoursbymonth(@q1year, 11, @projectid, 'State Share Volunteer') November on users.userid = November.userid
		left outer join udf_sumofhoursbymonth(@q1year, 12, @projectid, 'State Share Volunteer') December on users.userid = December.userid
		left outer join udf_sumofhoursbymonth(@year, 1, @projectid, 'State Share Volunteer') January on users.userid = January.userid
		left outer join udf_sumofhoursbymonth(@year, 2, @projectid, 'State Share Volunteer') February on users.userid = February.userid
		left outer join udf_sumofhoursbymonth(@year, 3, @projectid, 'State Share Volunteer') March on users.userid = March.userid
		left outer join udf_sumofhoursbymonth(@year, 4, @projectid, 'State Share Volunteer') April on users.userid = April.userid
		left outer join udf_sumofhoursbymonth(@year, 5, @projectid, 'State Share Volunteer') May on users.userid = May.userid
		left outer join udf_sumofhoursbymonth(@year, 6, @projectid, 'State Share Volunteer') June on users.userid = June.userid
		left outer join udf_sumofhoursbymonth(@year, 7, @projectid, 'State Share Volunteer') July on users.userid = July.userid
		left outer join udf_sumofhoursbymonth(@year, 8, @projectid, 'State Share Volunteer') August on users.userid = August.userid
		left outer join udf_sumofhoursbymonth(@year, 9, @projectid, 'State Share Volunteer') September on users.userid = September.userid
	--where users.isactive = 1
	group by users.firstname, users.lastname
	) totals
where (
	octoberhours <> 0
	or novemberhours <> 0
	or decemberhours <> 0
	or januaryhours <> 0
	or februaryhours <> 0
	or marchhours <> 0
	or aprilhours <> 0
	or mayhours <> 0
	or junehours <> 0
	or julyhours <> 0
	or augusthours <> 0
	or septemberhours <> 0
)
union
select *
from (
	select users.firstname, users.lastname, 'Teacher' FundType
		-- Quarter 1
	  , sum(IsNull(October.hours, 0)) OctoberHours, sum(IsNull(October.fte, 0)) OctoberFTE, sum(IsNull(october.pay, 0)) OctoberPay, sum(IsNull(october.benefits, 0)) OctoberBenefits, sum(IsNull(october.total, 0)) OctoberTotal, sum(IsNull(october.TotalIndr, 0)) octoberTotalIndr
	  , sum(IsNull(November.hours, 0)) NovemberHours, sum(IsNull(November.fte, 0)) NovemberFTE, sum(IsNull(november.pay, 0)) NovemberPay, sum(IsNull(november.benefits, 0)) NovemberBenefits, sum(IsNull(november.total, 0)) NovemberTotal, sum(IsNull(november.TotalIndr, 0)) novemberTotalIndr
	  , sum(IsNull(December.hours, 0)) DecemberHours, sum(IsNull(December.fte, 0)) DecemberFTE, sum(IsNull(December.pay, 0)) DecemberPay, sum(IsNull(December.benefits, 0)) DecemberBenefits, sum(IsNull(December.total, 0)) DecemberTotal, sum(IsNull(December.TotalIndr, 0)) DecemberTotalIndr
      
	  , sum(IsNull(October.hours, 0)) + sum(IsNull(November.hours, 0)) + sum(IsNull(December.hours, 0)) as Q1Hours
	  , (sum(IsNull(October.hours, 0)) + sum(IsNull(November.hours, 0)) + sum(IsNull(December.hours, 0))) / ( dbo.udf_QuarterTotalHours(@q1year, 'Q1') ) as Q1fte
	  , sum(IsNull(October.pay, 0)) + sum(IsNull(November.pay, 0)) + sum(IsNull(December.pay, 0)) as Q1pay, sum(IsNull(October.benefits, 0)) + sum(IsNull(November.benefits, 0)) + sum(IsNull(December.benefits, 0)) as Q1benefits
	  , sum(IsNull(October.total, 0)) + sum(IsNull(November.total, 0)) + sum(IsNull(December.total, 0)) as Q1total, sum(IsNull(October.TotalIndr, 0)) + sum(IsNull(November.TotalIndr, 0)) + sum(IsNull(December.TotalIndr, 0)) as Q1TotalIndr

		-- Quarter 2
	  , sum(IsNull(January.hours, 0)) JanuaryHours, sum(IsNull(January.fte, 0)) JanuaryFTE, sum(IsNull(January.pay, 0)) JanuaryPay, sum(IsNull(January.benefits, 0)) JanuaryBenefits, sum(IsNull(January.total, 0)) JanuaryTotal, sum(IsNull(January.TotalIndr, 0)) JanuaryTotalIndr
	  , sum(IsNull(February.hours, 0)) FebruaryHours, sum(IsNull(February.fte, 0)) FebruaryFTE, sum(IsNull(February.pay, 0)) FebruaryPay, sum(IsNull(February.benefits, 0)) FebruaryBenefits, sum(IsNull(February.total, 0)) FebruaryTotal, sum(IsNull(February.TotalIndr, 0)) FebruaryTotalIndr
	  , sum(IsNull(March.hours, 0)) MarchHours, sum(IsNull(March.fte, 0)) MarchFTE, sum(IsNull(March.pay, 0)) MarchPay, sum(IsNull(March.benefits, 0)) MarchBenefits, sum(IsNull(March.total, 0)) MarchTotal, sum(IsNull(March.TotalIndr, 0)) MarchTotalIndr
      
	  , sum(IsNull(January.hours, 0)) + sum(IsNull(February.hours, 0)) + sum(IsNull(March.hours, 0)) as Q2Hours
	  , (sum(IsNull(January.hours, 0)) + sum(IsNull(February.hours, 0)) + sum(IsNull(March.hours, 0))) / ( dbo.udf_QuarterTotalHours(@year, 'Q2') ) as Q2fte
	  , sum(IsNull(January.pay, 0)) + sum(IsNull(February.pay, 0)) + sum(IsNull(March.pay, 0)) as Q2pay, sum(IsNull(January.benefits, 0)) + sum(IsNull(February.benefits, 0)) + sum(IsNull(March.benefits, 0)) as Q2benefits
	  , sum(IsNull(January.total, 0)) + sum(IsNull(February.total, 0)) + sum(IsNull(March.total, 0)) as Q2total, sum(IsNull(January.TotalIndr, 0)) + sum(IsNull(February.TotalIndr, 0)) + sum(IsNull(March.TotalIndr, 0)) as Q2TotalIndr

		-- Quarter 3
	  , sum(IsNull(April.hours, 0)) AprilHours, sum(IsNull(April.fte, 0)) AprilFTE, sum(IsNull(April.pay, 0)) AprilPay, sum(IsNull(April.benefits, 0)) AprilBenefits, sum(IsNull(April.total, 0)) AprilTotal, sum(IsNull(April.TotalIndr, 0)) AprilTotalIndr
	  , sum(IsNull(May.hours, 0)) MayHours, sum(IsNull(May.fte, 0)) MayFTE, sum(IsNull(May.pay, 0)) MayPay, sum(IsNull(May.benefits, 0)) MayBenefits, sum(IsNull(May.total, 0)) MayTotal, sum(IsNull(May.TotalIndr, 0)) MayTotalIndr
	  , sum(IsNull(June.hours, 0)) JuneHours, sum(IsNull(June.fte, 0)) JuneFTE, sum(IsNull(June.pay, 0)) JunePay, sum(IsNull(June.benefits, 0)) JuneBenefits, sum(IsNull(June.total, 0)) JuneTotal, sum(IsNull(June.TotalIndr, 0)) JuneTotalIndr
      
	  , sum(IsNull(April.hours, 0)) + sum(IsNull(May.hours, 0)) + sum(IsNull(June.hours, 0)) as Q3Hours
	  , (sum(IsNull(April.hours, 0)) + sum(IsNull(May.hours, 0)) + sum(IsNull(June.hours, 0))) / ( dbo.udf_QuarterTotalHours(@year, 'Q3') ) as Q3fte
	  , sum(IsNull(April.pay, 0)) + sum(IsNull(May.pay, 0)) + sum(IsNull(June.pay, 0)) as Q3pay, sum(IsNull(April.benefits, 0)) + sum(IsNull(May.benefits, 0)) + sum(IsNull(June.benefits, 0)) as Q3benefits
	  , sum(IsNull(April.total, 0)) + sum(IsNull(May.total, 0)) + sum(IsNull(June.total, 0)) as Q3total, sum(IsNull(April.TotalIndr, 0)) + sum(IsNull(May.TotalIndr, 0)) + sum(IsNull(June.TotalIndr, 0)) as Q3TotalIndr
		
		-- Quarter 4
	  , sum(IsNull(July.hours, 0)) JulyHours, sum(IsNull(July.fte, 0)) JulyFTE, sum(IsNull(July.pay, 0)) JulyPay, sum(IsNull(July.benefits, 0)) JulyBenefits, sum(IsNull(July.total, 0)) JulyTotal, sum(IsNull(July.TotalIndr, 0)) JulyTotalIndr
	  , sum(IsNull(August.hours, 0)) AugustHours, sum(IsNull(August.fte, 0)) AugustFTE, sum(IsNull(August.pay, 0)) AugustPay, sum(IsNull(August.benefits, 0)) AugustBenefits, sum(IsNull(August.total, 0)) AugustTotal, sum(IsNull(August.TotalIndr, 0)) AugustTotalIndr
	  , sum(IsNull(September.hours, 0)) SeptemberHours, sum(IsNull(September.fte, 0)) SeptemberFTE, sum(IsNull(September.pay, 0)) SeptemberPay, sum(IsNull(September.benefits, 0)) SeptemberBenefits, sum(IsNull(September.total, 0)) SeptemberTotal, sum(IsNull(September.TotalIndr, 0)) SeptemberTotalIndr
      
	  , sum(IsNull(July.hours, 0)) + sum(IsNull(August.hours, 0)) + sum(IsNull(September.hours, 0)) as Q4Hours
	  , (sum(IsNull(July.hours, 0)) + sum(IsNull(August.hours, 0)) + sum(IsNull(September.hours, 0))) / ( dbo.udf_QuarterTotalHours(@year, 'Q4') ) as Q4fte
	  , sum(IsNull(July.pay, 0)) + sum(IsNull(August.pay, 0)) + sum(IsNull(September.pay, 0)) as Q4pay, sum(IsNull(July.benefits, 0)) + sum(IsNull(August.benefits, 0)) + sum(IsNull(September.benefits, 0)) as Q4benefits
	  , sum(IsNull(July.total, 0)) + sum(IsNull(August.total, 0)) + sum(IsNull(September.total, 0)) as Q4total, sum(IsNull(July.TotalIndr, 0)) + sum(IsNull(August.TotalIndr, 0)) + sum(IsNull(September.TotalIndr, 0)) as Q4TotalIndr
	  
	  , [dbo].[udf_QuarterTotalHours] (@year, 'FN') as FinalHours
	  
	from users
		left outer join udf_sumofhoursbymonth(@q1year, 10, @projectid, '3rd Party Teachers') October on users.userid = October.userid
		left outer join udf_sumofhoursbymonth(@q1year, 11, @projectid, '3rd Party Teachers') November on users.userid = November.userid
		left outer join udf_sumofhoursbymonth(@q1year, 12, @projectid, '3rd Party Teachers') December on users.userid = December.userid
		left outer join udf_sumofhoursbymonth(@year, 1, @projectid, '3rd Party Teachers') January on users.userid = January.userid
		left outer join udf_sumofhoursbymonth(@year, 2, @projectid, '3rd Party Teachers') February on users.userid = February.userid
		left outer join udf_sumofhoursbymonth(@year, 3, @projectid, '3rd Party Teachers') March on users.userid = March.userid
		left outer join udf_sumofhoursbymonth(@year, 4, @projectid, '3rd Party Teachers') April on users.userid = April.userid
		left outer join udf_sumofhoursbymonth(@year, 5, @projectid, '3rd Party Teachers') May on users.userid = May.userid
		left outer join udf_sumofhoursbymonth(@year, 6, @projectid, '3rd Party Teachers') June on users.userid = June.userid
		left outer join udf_sumofhoursbymonth(@year, 7, @projectid, '3rd Party Teachers') July on users.userid = July.userid
		left outer join udf_sumofhoursbymonth(@year, 8, @projectid, '3rd Party Teachers') August on users.userid = August.userid
		left outer join udf_sumofhoursbymonth(@year, 9, @projectid, '3rd Party Teachers') September on users.userid = September.userid
	--where users.isactive = 1
	group by users.firstname, users.lastname
) totals
where (
	octoberhours <> 0
	or novemberhours <> 0
	or decemberhours <> 0
	or januaryhours <> 0
	or februaryhours <> 0
	or marchhours <> 0
	or aprilhours <> 0
	or mayhours <> 0
	or junehours <> 0
	or julyhours <> 0
	or augusthours <> 0
	or septemberhours <> 0
)
order by FundType, lastname


GO

GRANT EXEC ON usp_GenerateTimeCostShare TO PUBLIC

GO
