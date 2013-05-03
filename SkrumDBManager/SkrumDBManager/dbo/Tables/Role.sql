CREATE TABLE [dbo].[Role] (
    [RoleID]       INT        IDENTITY (1, 1) NOT NULL,
    [Description]  INT NOT NULL,
    [AssignedTime] FLOAT (53) NOT NULL,
    [ProjectID]    INT        NOT NULL,
    [PersonID]     INT        NOT NULL,
    [Password] VARCHAR(128) NULL, 
    PRIMARY KEY CLUSTERED ([RoleID] ASC),
    CONSTRAINT [FK_Role_ToPerson] FOREIGN KEY ([PersonID]) REFERENCES [dbo].[Person] ([PersonID]),
    CONSTRAINT [FK_Role_ToProject] FOREIGN KEY ([ProjectID]) REFERENCES [dbo].[Project] ([ProjectID]), 
    CONSTRAINT [FK_Role_ToRoleDescription] FOREIGN KEY ([Description]) REFERENCES [dbo].[RoleDescription] ([RoleDescriptionID])
);

