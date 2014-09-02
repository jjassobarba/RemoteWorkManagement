/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/
PRINT 'Inserting Roles Catalog'

INSERT INTO Roles SELECT 'Admin','RemoteWorkManagement' WHERE NOT EXISTS (SELECT 1 FROM Roles WHERE Id = 1)
INSERT INTO Roles SELECT 'TeamLeader','RemoteWorkManagement' WHERE NOT EXISTS (SELECT 2 FROM Roles WHERE Id = 2)
INSERT INTO Roles SELECT 'TeamMember','RemoteWorkManagement' WHERE NOT EXISTS (SELECT 3 FROM Roles WHERE Id = 3)
INSERT INTO Roles SELECT 'Sensei','RemoteWorkManagement' WHERE NOT EXISTS (SELECT 4 FROM Roles WHERE Id = 4)

PRINT 'inserting User'

insert into users(UserName,ApplicationName,Email,PassWord,Comment,PasswordQuestion,PasswordAnswer,IsApproved,
LastActivityDate,LastLoginDate,LastPasswordChangedDate,CreationDate,IsOnLine,IsLockedOut,LastLockedOutDate,FailedPasswordAttemptCount,
FailedPasswordAttemptWindowStart,FailedPasswordAnswerAttemptCount,FailedPasswordAnswerAttemptWindowStart)  
values('jdromo@sciodev.com','RemoteWorkManagement','jdromo@sciodev.com',HashBytes('MD5', 'admin'),'','',HashBytes('MD5', 'rpta'),1,
GETDATE(),GETDATE(),GETDATE(),GETDATE(),0,0,GETDATE(),0,
GETDATE(),0,GETDATE());

insert into UserInfo(IdUserInfo,IdMembership,FirstName,LastName,IsTemporalPassword)
values (NEWID(),1,'AdminName','AdminLstName',0);