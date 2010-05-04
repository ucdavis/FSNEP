USE [FSNEPv2]

DECLARE @dayInt int
DECLARE @nowMinusOneMonth datetime
DECLARE @previousMonthInt int
DECLARE @previousYearInt int

SET @dayInt = (SELECT DATEPART(DAY, GETDATE()))

SET @nowMinusOneMonth = (SELECT DATEADD(month, -1, GETDATE()) as PrevMonth)
SET @previousMonthInt = (SELECT DATEPART(month, @nowMinusOneMonth))
SET @previousYearInt = (SELECT DATEPART(year, @nowMinusOneMonth))

SELECT @previousMonthInt, @previousYearInt

DECLARE @MailList CURSOR
SET @MailList = CURSOR FOR

--Get all of the FTE TimeSheet Users who should have submitted a time record
SELECT     aspnet_Membership.Email, Users.FirstName + ' ' + Users.LastName as FullName, sup.Email
FROM         aspnet_Membership INNER JOIN
                      Users ON aspnet_Membership.UserId = Users.UserId INNER JOIN
                      aspnet_UsersInRoles ON aspnet_Membership.UserId = aspnet_UsersInRoles.UserId INNER JOIN
                      aspnet_Membership AS sup ON sup.UserId = Users.SupervisorID INNER JOIN
                      aspnet_Roles ON aspnet_UsersInRoles.RoleId = aspnet_Roles.RoleId 
WHERE     (aspnet_Roles.RoleName = N'Timesheet User') AND (Users.IsActive = 1)
		AND aspnet_Membership.UserId NOT IN (
			--Remove those with submitted time records for the previous month
			SELECT     members.UserId
			FROM         Records AS Rec INNER JOIN
								  Users ON Users.UserId = Rec.UserId INNER JOIN
								  aspnet_Membership AS members ON members.UserId = Rec.UserId INNER JOIN
								  aspnet_Membership AS sup ON sup.UserId = Users.SupervisorID INNER JOIN
								  Status ON Status.ID = Rec.StatusID INNER JOIN
								  TimeRecords ON Rec.ID = TimeRecords.ID
			WHERE     (Rec.Month = @previousMonthInt) AND (Rec.Year = @previousYearInt)
		)

OPEN @MailList

DECLARE @Email varchar(50), @FullName varchar(100), @SupervisorEmail varchar(50)
FETCH NEXT FROM @MailList INTO @Email, @FullName, @SupervisorEmail

WHILE (@@FETCH_STATUS = 0)
	BEGIN
		--Send Emails on some date (TBD) to those who should have submitted a time record and didn't 
		DECLARE @bodyText varchar(MAX)
		
		SET @bodyText = 'Dear ' + @FullName + '. This is the body of the test message.
				Database Mail Received By you Successfully.'

		EXEC msdb.dbo.sp_send_dbmail
			@recipients=@Email
			@subject='FSNEP Time Record Submit Reminder',
			@body=@bodyText
 
		FETCH NEXT FROM @MailList INTO @Email, @FullName, @SupervisorEmail
	END

CLOSE @MailList
DEALLOCATE @MailList