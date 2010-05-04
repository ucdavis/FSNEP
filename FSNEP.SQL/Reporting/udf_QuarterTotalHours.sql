 IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[udf_QuarterTotalHours]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
	DROP FUNCTION [dbo].[udf_QuarterTotalHours]
GO

CREATE FUNCTION [dbo].[udf_QuarterTotalHours]
(
	-- Add the parameters for the function here
	@year int,
	@quarter char(2)
)
RETURNS int
AS
BEGIN

-- Declare the return variable here
	DECLARE @result int
	
	IF @quarter = 'Q1'
			SELECT @result = SUM(HoursInMonth) 
			FROM HoursInMonths 
			WHERE [Year] = @year AND [Month] IN (10, 11, 12)
	ELSE IF @quarter = 'Q2' 
			SELECT @result = SUM(HoursInMonth) 
			FROM HoursInMonths 
			WHERE [Year] = @year AND [Month] IN (1, 2, 3)
	ELSE IF @quarter = 'Q3' 
			SELECT @result = SUM(HoursInMonth) 
			FROM HoursInMonths 
			WHERE [Year] = @year AND [Month] IN (4, 5, 6)
	ELSE IF @quarter = 'Q4' 
			SELECT @result = SUM(HoursInMonth) 
			FROM HoursInMonths 
			WHERE [Year] = @year AND [Month] IN (7, 8, 9)
	ELSE
		SELECT @result = SUM(HoursInMonth) 
			FROM HoursInMonths
			WHERE ([Year] = @year - 1 AND [Month] IN (10, 11, 12))
					OR ([Year] = @year AND [Month] NOT IN (10, 11, 12))
							
	-- Return the result of the function
	RETURN ISNULL(@result, -1)

END

GO
