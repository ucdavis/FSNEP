﻿IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'usp_NotifyTimeRecordUsers')
	BEGIN
		DROP  Procedure  usp_NotifyTimeRecordUsers
	END

GO

CREATE Procedure usp_NotifyTimeRecordUsers

AS

DECLARE @dayInt int
DECLARE @nowMinusOneMonth datetime
DECLARE @previousMonthInt int
DECLARE @previousYearInt int

SET @dayInt = (SELECT DATEPART(DAY, GETDATE()))

SET @nowMinusOneMonth = (SELECT DATEADD(month, -1, GETDATE()) as PrevMonth)
SET @previousMonthInt = (SELECT DATEPART(month, @nowMinusOneMonth))
SET @previousYearInt = (SELECT DATEPART(year, @nowMinusOneMonth))

IF (@dayInt <> 7)
BEGIN
	print 'Only notify time record users on the 7th of each month'
	RETURN
END

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
						AND (Status.Name = 'PendingReview' OR Status.Name = 'Approved') --Only submitted if status on of thesez
		)

OPEN @MailList

DECLARE @Email varchar(50), @FullName varchar(100), @SupervisorEmail varchar(50)
FETCH NEXT FROM @MailList INTO @Email, @FullName, @SupervisorEmail

WHILE (@@FETCH_STATUS = 0)
	BEGIN
		--Send Emails on some date (TBD) to those who should have submitted a time record and didn't 
		DECLARE @bodyText varchar(MAX)
		
		SET @bodyText = 'This email was generated by the UC-CalFresh Nutrition Education Program Online Time Record System.
*** Please do not respond to this email address ***

This is a reminder that your electronically certified UC-CalFresh Nutrition Education Program time record is due on the 15th of this month.  Please access the UC-CalFresh Nutrition Education Program Online Time Record system to submit your time record.

If you have any questions regarding this message, or about time records in general, please contact fsnep_support@ucdavis.edu.

UC-CalFresh Nutrition Education Program Online Record System

https://secure.caes.ucdavis.edu/FSNEPRecords
'

		EXEC msdb.dbo.sp_send_dbmail
			@recipients=@Email,
			@subject='UC-CalFresh Nutrition Education Program Time Record Due Reminder',
			@body=@bodyText
 
		FETCH NEXT FROM @MailList INTO @Email, @FullName, @SupervisorEmail
	END

CLOSE @MailList
DEALLOCATE @MailList

GO

GRANT EXEC ON usp_NotifyTimeRecordUsers TO PUBLIC

GO


