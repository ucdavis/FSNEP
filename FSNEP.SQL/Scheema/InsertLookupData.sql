
SET IDENTITY_INSERT [dbo].[Status] ON
INSERT [dbo].[Status] ([ID], [Name]) VALUES (1, N'Current')
INSERT [dbo].[Status] ([ID], [Name]) VALUES (2, N'PendingReview')
INSERT [dbo].[Status] ([ID], [Name]) VALUES (3, N'Approved')
INSERT [dbo].[Status] ([ID], [Name]) VALUES (4, N'Disapproved')
SET IDENTITY_INSERT [dbo].[Status] OFF

INSERT [dbo].[AuditActionTypes] ([ID], [ActionCodeName]) VALUES (N'C', N'Create')
INSERT [dbo].[AuditActionTypes] ([ID], [ActionCodeName]) VALUES (N'D', N'Delete')
INSERT [dbo].[AuditActionTypes] ([ID], [ActionCodeName]) VALUES (N'U', N'Update')
 