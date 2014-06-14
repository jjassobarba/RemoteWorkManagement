
--CREATE AN NEW DATABSE OR USE AN EXISTING DATABASE

--NEW DATABASE 
USE [master]
GO

CREATE DATABASE FNHProviderDB
GO

USE [FNHProviderDB]
GO

---USERS table
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

SET ANSI_PADDING OFF
GO

--Roles table 
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

SET ANSI_PADDING OFF
GO

---usersinroles table

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

---create a few sample roles ..note application name can be anything you want but make sure you use the same in the web.confing file as well


insert into roles(RoleName,ApplicationName)
values('Admin','FNHApplication')
GO

insert into roles(RoleName,ApplicationName)
values('Editor','FNHApplication')
GO
