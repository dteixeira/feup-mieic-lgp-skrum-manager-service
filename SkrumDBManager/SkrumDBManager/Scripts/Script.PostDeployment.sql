﻿/*
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
INSERT INTO [dbo].[Person] ([Name], [PhotoURL], [Email], [Admin], [JobDescription], [Password])
	VALUES ('Default Admin', null, 'admin@default.com', 1, null, '123456');

/* StoryState enumaration values */
INSERT INTO [dbo].[StoryState] ([State]) VALUES ('InProgress');
INSERT INTO [dbo].[StoryState] ([State]) VALUES ('Completed');
INSERT INTO [dbo].[StoryState] ([State]) VALUES ('Abandoned');

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