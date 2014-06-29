CREATE TABLE [dbo].[UserInfo]
(
	[IdUserInfo] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
    [IdMembership] INT NOT NULL, 
    [FirstName] VARCHAR(100) NOT NULL, 
    [LastName] VARCHAR(100) NOT NULL, 
    [Position] VARCHAR(50) NULL, 
    [ProjectLeader] VARCHAR(50) NULL, 
    [RemoteDays] VARCHAR(100) NULL, 
    [FlexTime] VARCHAR(50) NULL, 
    [Picture] VARBINARY(MAX) NULL, 
    CONSTRAINT [FK_UserInfo_Users] FOREIGN KEY ([IdMembership]) REFERENCES [Users]([Id])
)
