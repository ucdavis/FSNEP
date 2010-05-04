/* ASP.NET Scheema Provider Information */
INSERT [dbo].[aspnet_SchemaVersions] ([Feature], [CompatibleSchemaVersion], [IsCurrentVersion]) VALUES (N'common', N'1', 1)
INSERT [dbo].[aspnet_SchemaVersions] ([Feature], [CompatibleSchemaVersion], [IsCurrentVersion]) VALUES (N'health monitoring', N'1', 1)
INSERT [dbo].[aspnet_SchemaVersions] ([Feature], [CompatibleSchemaVersion], [IsCurrentVersion]) VALUES (N'membership', N'1', 1)
INSERT [dbo].[aspnet_SchemaVersions] ([Feature], [CompatibleSchemaVersion], [IsCurrentVersion]) VALUES (N'personalization', N'1', 1)
INSERT [dbo].[aspnet_SchemaVersions] ([Feature], [CompatibleSchemaVersion], [IsCurrentVersion]) VALUES (N'profile', N'1', 1)
INSERT [dbo].[aspnet_SchemaVersions] ([Feature], [CompatibleSchemaVersion], [IsCurrentVersion]) VALUES (N'role manager', N'1', 1)

/* Application Name */
INSERT [dbo].[aspnet_Applications] ([ApplicationName], [LoweredApplicationName], [ApplicationId], [Description]) VALUES (N'FSNEP', N'fsnep', N'1685dda8-60a0-4b40-8da4-da176be790d0', NULL)

/* Roles */
INSERT [dbo].[aspnet_Roles] ([ApplicationId], [RoleId], [RoleName], [LoweredRoleName], [Description]) VALUES (N'1685dda8-60a0-4b40-8da4-da176be790d0', N'5b7574f3-1d86-4933-b059-e1a621dc425d', N'DelegateSupervisor', N'delegatesupervisor', NULL)
INSERT [dbo].[aspnet_Roles] ([ApplicationId], [RoleId], [RoleName], [LoweredRoleName], [Description]) VALUES (N'1685dda8-60a0-4b40-8da4-da176be790d0', N'c84e44fc-9762-4977-b509-86ac53998e27', N'NonPayrollCostShareExpenses', N'nonpayrollcostshareexpenses', NULL)
INSERT [dbo].[aspnet_Roles] ([ApplicationId], [RoleId], [RoleName], [LoweredRoleName], [Description]) VALUES (N'1685dda8-60a0-4b40-8da4-da176be790d0', N'f640f95d-511d-43df-89c0-d2a94444c02c', N'ProjectAdmin', N'projectadmin', NULL)
INSERT [dbo].[aspnet_Roles] ([ApplicationId], [RoleId], [RoleName], [LoweredRoleName], [Description]) VALUES (N'1685dda8-60a0-4b40-8da4-da176be790d0', N'e59d6063-b007-4b75-9152-5aa76abdd183', N'State Office', N'state office', NULL)
INSERT [dbo].[aspnet_Roles] ([ApplicationId], [RoleId], [RoleName], [LoweredRoleName], [Description]) VALUES (N'1685dda8-60a0-4b40-8da4-da176be790d0', N'98e574a8-d603-43c3-9c62-6f849f482267', N'Supervisor', N'supervisor', NULL)
INSERT [dbo].[aspnet_Roles] ([ApplicationId], [RoleId], [RoleName], [LoweredRoleName], [Description]) VALUES (N'1685dda8-60a0-4b40-8da4-da176be790d0', N'26898590-9bc5-4a73-b64d-09efb0140b6e', N'Timesheet User', N'timesheet user', NULL)