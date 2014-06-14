CREATE TABLE [dbo].[UsersInRoles](
	[Users_Id] [int] NOT NULL,
	[Roles_Id] [int] NOT NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[UsersInRoles]  WITH CHECK ADD  CONSTRAINT [FK_UsersInRoles_Roles] FOREIGN KEY([Roles_Id])
REFERENCES [dbo].[Roles] ([Id])
GO

ALTER TABLE [dbo].[UsersInRoles] CHECK CONSTRAINT [FK_UsersInRoles_Roles]
GO
ALTER TABLE [dbo].[UsersInRoles]  WITH CHECK ADD  CONSTRAINT [FK_UsersInRoles_Users] FOREIGN KEY([Users_Id])
REFERENCES [dbo].[Users] ([Id])
GO

ALTER TABLE [dbo].[UsersInRoles] CHECK CONSTRAINT [FK_UsersInRoles_Users]