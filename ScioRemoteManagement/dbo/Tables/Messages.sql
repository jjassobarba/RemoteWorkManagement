﻿CREATE TABLE [dbo].[Messages]
(
	[IdMessage] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
    [IdUserInfo] UNIQUEIDENTIFIER NOT NULL, 
    [IdTo] UNIQUEIDENTIFIER NOT NULL, 
    [Message] VARCHAR(500) NOT NULL, 
    CONSTRAINT [FK_Messages_UserInfo] FOREIGN KEY ([IdUserInfo]) REFERENCES [UserInfo]([IdUserInfo])
)
