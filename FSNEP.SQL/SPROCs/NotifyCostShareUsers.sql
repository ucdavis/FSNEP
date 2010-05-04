--Note: Not required in current version
/*
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

--Get those with submitted cost shares for the previous month
SELECT     members.Email, Users.FirstName + ' ' + Users.LastName AS FullName, sup.Email AS SupervisorEmail
FROM         Records AS Rec INNER JOIN
                      Users ON Users.UserId = Rec.UserId INNER JOIN
                      aspnet_Membership AS members ON members.UserId = Rec.UserId INNER JOIN
                      aspnet_Membership AS sup ON sup.UserId = Users.SupervisorID INNER JOIN
                      Status ON Status.ID = Rec.StatusID INNER JOIN
                      CostShareRecords ON Rec.ID = CostShareRecords.ID
WHERE     (Rec.Month = @previousMonthInt) AND (Rec.Year = @previousYearInt) AND (Status.Name = 'Approved' OR
                      Status.Name = 'PendingReview')

--Get all of the FTE CostShare Users who should have submitted a cost share
SELECT     aspnet_Membership.Email, Users.FirstName, Users.LastName, sup.Email
FROM         aspnet_Membership INNER JOIN
                      Users ON aspnet_Membership.UserId = Users.UserId INNER JOIN
                      aspnet_UsersInRoles ON aspnet_Membership.UserId = aspnet_UsersInRoles.UserId INNER JOIN
                      aspnet_Membership AS sup ON sup.UserId = Users.SupervisorID INNER JOIN
                      aspnet_Roles ON aspnet_UsersInRoles.RoleId = aspnet_Roles.RoleId 
WHERE     (aspnet_Roles.RoleName = N'NonPayrollCostShareExpenses') AND (Users.IsActive = 1)

--Send Emails on some date (TBD) to those who should have submitted a time record and didn't 
*/