CREATE TABLE [dbo].[Outbox]
(
	[IdOutbox] UNIQUEIDENTIFIER NOT NULL , 	
	[IdUserInfo] UNIQUEIDENTIFIER NOT NULL , 	
    [IdMessage] UNIQUEIDENTIFIER NOT NULL, 
    [IsForwarded] BIT NULL, 
    CONSTRAINT [PK_Outbox] PRIMARY KEY ([IdOutbox]),     
    CONSTRAINT [FK_Outbox_UserInfo] FOREIGN KEY ([IdUserInfo]) REFERENCES [UserInfo]([IdUserInfo]),
	CONSTRAINT [FK_Outbox_Messages] FOREIGN KEY ([IdMessage]) REFERENCES [Messages]([IdMessage]) 
)
