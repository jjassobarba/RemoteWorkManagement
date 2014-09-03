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

PRINT 'Inserting User'

INSERT INTO Users SELECT 'jdromo@sciodev.com','RemoteWorkManagement','jdromo@sciodev.com','',HashBytes('SHA1', 'admin'),'',HashBytes('SHA1', 'rpta'),1,
GETDATE(),GETDATE(),GETDATE(),GETDATE(),0,0,GETDATE(),0,GETDATE(),0,GETDATE() WHERE NOT EXISTS (SELECT 1 FROM Users WHERE Id=1);

INSERT INTO UserInfo SELECT NEWID(),1,'AdminName','AdminLstName','',null,null,null,null,null,1,0,null WHERE NOT EXISTS (SELECT 1 FROM UserInfo WHERE IdMembership=1);