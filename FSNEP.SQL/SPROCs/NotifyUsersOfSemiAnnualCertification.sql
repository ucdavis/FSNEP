USE [FSNEPv2]

DECLARE @dayInt int
DECLARE @monthInt int

SET @dayInt = (SELECT DATEPART(DAY, GETDATE()))
SET @monthInt = (SELECT DATEPART(month, GETDATE()))

IF (@dayInt = 13) AND (@monthInt = 4 OR @monthInt = 1)
BEGIN

	DECLARE @MailList CURSOR
	SET @MailList = CURSOR FOR

	--Get all of the 1.0 FTE TimeSheet Users
	SELECT     aspnet_Membership.Email, Users.FirstName + ' ' + Users.LastName as FullName
	FROM         aspnet_Membership INNER JOIN
						  aspnet_UsersInRoles ON aspnet_Membership.UserId = aspnet_UsersInRoles.UserId INNER JOIN
						  aspnet_Roles ON aspnet_UsersInRoles.RoleId = aspnet_Roles.RoleId AND aspnet_UsersInRoles.RoleId = aspnet_Roles.RoleId INNER JOIN
						  Users ON aspnet_Membership.UserId = Users.UserId
	WHERE     (aspnet_Roles.RoleName = N'Timesheet User') AND (FTE = 1)

	OPEN @MailList

	DECLARE @Email varchar(50), @FullName varchar(100)
	FETCH NEXT FROM @MailList INTO @Email, @FullName

	WHILE (@@FETCH_STATUS = 0)
		BEGIN
			--Send emails to each matching user
			DECLARE @bodyText varchar(MAX)

			SET @bodyText = 'Dear ' + @FullName + '. This is the body of the test message.
			Database Mail Received By you Successfully.'
			
			EXEC msdb.dbo.sp_send_dbmail
			@recipients=@Email
			@subject='Test message',
			@body=@bodyText
			
			FETCH NEXT FROM @MailList INTO @Email, @FullName
		END	

	CLOSE @MailList
	DEALLOCATE @MailList

END