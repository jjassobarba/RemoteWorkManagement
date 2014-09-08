CREATE TABLE [dbo].[CheckInOut]
(
	[IdCheck] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
    [IdUserInfo] UNIQUEIDENTIFIER NOT NULL, 
    [CheckInDate] DATETIME NOT NULL, 
    [CheckOutDate] DATETIME NULL, 
	[IsManualCheckIn] BIT NULL,
	[IsManualCheckOut] BIT Null,
    CONSTRAINT [FK_CheckInOut_UserInfo] FOREIGN KEY ([IdUserInfo]) REFERENCES [UserInfo]([IdUserInfo])
)
