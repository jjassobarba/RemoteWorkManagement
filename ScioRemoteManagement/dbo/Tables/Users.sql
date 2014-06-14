CREATE TABLE [dbo].[Users](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Username] [varchar](64) NOT NULL,
	[ApplicationName] [varchar](64) NOT NULL,
	[Email] [varchar](128) NOT NULL,
	[Comment] [varchar](255) NULL,
	[Password] [varchar](64) NOT NULL,
	[PasswordQuestion] [varchar](255) NULL,
	[PasswordAnswer] [varchar](255) NULL,
	[IsApproved] [bit] NULL,
	[LastActivityDate] [datetime] NULL,
	[LastLoginDate] [datetime] NULL,
	[LastPasswordChangedDate] [datetime] NULL,
	[CreationDate] [datetime] NULL,
	[IsOnLine] [bit] NULL,
	[IsLockedOut] [bit] NULL,
	[LastLockedOutDate] [datetime] NULL,
	[FailedPasswordAttemptCount] [int] NULL,
	[FailedPasswordAttemptWindowStart] [datetime] NULL,
	[FailedPasswordAnswerAttemptCount] [int] NULL,
	[FailedPasswordAnswerAttemptWindowStart] [datetime] NULL,
 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]