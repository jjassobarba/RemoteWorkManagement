CREATE TABLE [dbo].[Inbox]
(
	[IdInbox] UNIQUEIDENTIFIER NOT NULL, 	
	[IdUserInfo] UNIQUEIDENTIFIER NOT NULL, 	
    [IdMessage] UNIQUEIDENTIFIER NOT NULL,     
    [IsForwarded] BIT NULL, 
    CONSTRAINT [PK_Inbox] PRIMARY KEY ([IdInbox]),     
    CONSTRAINT [FK_Inbox_UserInfo] FOREIGN KEY ([IdUserInfo]) REFERENCES [UserInfo]([IdUserInfo]),
    CONSTRAINT [FK_Inbox_Messages] FOREIGN KEY ([IdMessage]) REFERENCES [Messages]([IdMessage]) 
)
