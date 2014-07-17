CREATE TABLE [dbo].[Notifications]
(
	[IdNotification] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
    [IdUserInfo] UNIQUEIDENTIFIER NOT NULL, 
    [ProjectLeaderMail] VARCHAR(200) NULL, 
    [TeamMail] VARCHAR(200) NULL, 
    [OtherMails] VARCHAR(MAX) NULL, 
    CONSTRAINT [FK_Notifications_UserInfo] FOREIGN KEY ([IdUserInfo]) REFERENCES [UserInfo]([IdUserInfo])
)
