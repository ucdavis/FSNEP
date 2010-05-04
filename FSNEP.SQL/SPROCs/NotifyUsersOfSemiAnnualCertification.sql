USE [FSNEPv2]

DECLARE @dayInt int
DECLARE @monthInt int

SET @dayInt = (SELECT DATEPART(DAY, GETDATE()))
SET @monthInt = (SELECT DATEPART(month, GETDATE()))

IF @dayInt = 7 --Only continue if it is the 1st of the month
BEGIN

	--Only continue if it is april or october
	IF @monthInt = 4 OR @monthInt = 10
	BEGIN

		SELECT @dayInt, @monthInt

		--Get all of the 1.0 FTE TimeSheet Users
		SELECT     aspnet_Membership.Email, Users.FirstName, Users.LastName
		FROM         aspnet_Membership INNER JOIN
							  aspnet_UsersInRoles ON aspnet_Membership.UserId = aspnet_UsersInRoles.UserId INNER JOIN
							  aspnet_Roles ON aspnet_UsersInRoles.RoleId = aspnet_Roles.RoleId AND aspnet_UsersInRoles.RoleId = aspnet_Roles.RoleId INNER JOIN
							  Users ON aspnet_Membership.UserId = Users.UserId
		WHERE     (aspnet_Roles.RoleName = N'Timesheet User') AND (FTE = 1)

		--Send them all emails
	END
END 