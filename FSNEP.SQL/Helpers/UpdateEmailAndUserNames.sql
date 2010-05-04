/* Update the email/username quick lookup columns from the aspnet_xxx tables */
update FSNEP2.dbo.Users
set Email = (
	select membership.Email
	from FSNEP.dbo.aspnet_Membership membership
	where Users.UserId = membership.UserId
)

update FSNEP2.dbo.Users
set UserName = (
	select u.UserName
	from FSNEP.dbo.aspnet_Users u
	where Users.UserId = u.UserId
)