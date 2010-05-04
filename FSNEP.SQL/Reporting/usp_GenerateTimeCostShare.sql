IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'usp_GenerateTimeCostShare')
	BEGIN
		DROP  Procedure  usp_GenerateTimeCostShare
	END

GO

CREATE Procedure usp_GenerateTimeCostShare
/*
	(
		@parameter1 int = 5,
		@parameter2 datatype OUTPUT
	)

*/
AS


GO

/*
GRANT EXEC ON usp_GenerateTimeCostShare TO PUBLIC

GO
*/

