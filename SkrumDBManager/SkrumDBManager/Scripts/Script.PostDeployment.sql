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

/* Default admin */
INSERT INTO [dbo].[Person] ([Name], [PhotoURL], [Email], [JobDescription], [Password])
	VALUES ('Default Admin', null, 'admin@default.com', null, 'ba3253876aed6bc22d4a6ff53d8406c6ad864195ed144ab5c87621b6c233b548baeae6956df346ec8c17f5ea10f35ee3cbc514797ed7ddd3145464e2a0bab413');

/* StoryState enumaration values */
INSERT INTO [dbo].[StoryState] ([State]) VALUES ('InProgress');
INSERT INTO [dbo].[StoryState] ([State]) VALUES ('Completed');
INSERT INTO [dbo].[StoryState] ([State]) VALUES ('Abandoned');

/* TaskState enumaration values */
INSERT INTO [dbo].[TaskState] ([State]) VALUES ('Waiting');
INSERT INTO [dbo].[TaskState] ([State]) VALUES ('InProgress');
INSERT INTO [dbo].[TaskState] ([State]) VALUES ('Testing');
INSERT INTO [dbo].[TaskState] ([State]) VALUES ('Completed');

/* StoryPriority enumaration values */
INSERT INTO [dbo].[StoryPriority] ([Priority]) VALUES ('Must');
INSERT INTO [dbo].[StoryPriority] ([Priority]) VALUES ('Should');
INSERT INTO [dbo].[StoryPriority] ([Priority]) VALUES ('Could');
INSERT INTO [dbo].[StoryPriority] ([Priority]) VALUES ('Wont');

/* RoleDescription enumeration values */
INSERT INTO [dbo].[RoleDescription] ([Description]) VALUES ('ProjectManager');
INSERT INTO [dbo].[RoleDescription] ([Description]) VALUES ('ScrumMaster');
INSERT INTO [dbo].[RoleDescription] ([Description]) VALUES ('ProductOwner');
INSERT INTO [dbo].[RoleDescription] ([Description]) VALUES ('TeamMember');