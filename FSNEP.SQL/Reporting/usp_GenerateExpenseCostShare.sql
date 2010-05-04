IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'usp_GenerateExpenseCostShare')
	BEGIN
		DROP  Procedure  usp_GenerateExpenseCostShare
	END

GO

CREATE Procedure usp_GenerateExpenseCostShare
/*
	(
		@parameter1 int = 5,
		@parameter2 datatype OUTPUT
	)

*/
AS


GO

/*
GRANT EXEC ON usp_GenerateExpenseCostShare TO PUBLIC

GO
*/

