CREATE TABLE [dbo].[UserInfo]
(
	[IdUserInfo] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
    [IdMembership] INT NOT NULL, 
    [FirstName] VARCHAR(100) NOT NULL, 
    [LastName] VARCHAR(100) NOT NULL, 
    [Position] VARCHAR(50) NULL, 
    [IdProjectLeader] UNIQUEIDENTIFIER NULL, 
    [RemoteDays] VARCHAR(100) NULL, 
    [FlexTime] VARCHAR(50) NULL, 
    [Picture] VARBINARY(MAX) NULL, 
    [OtherFlexTime] VARCHAR(200) NULL, 
    [ReceiveNotifications] BIT NULL, 
    [IsTemporalPassword] BIT NULL DEFAULT 1, 
    [IdSensei] UNIQUEIDENTIFIER NULL, 
    CONSTRAINT [FK_UserInfo_Users] FOREIGN KEY ([IdMembership]) REFERENCES [Users]([Id])
)
