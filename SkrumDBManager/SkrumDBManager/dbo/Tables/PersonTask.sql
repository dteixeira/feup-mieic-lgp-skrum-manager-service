CREATE TABLE [dbo].[PersonTask] (
    [TaskID]       INT NOT NULL,
    [PersonID]     INT NOT NULL,
    [SpentTime]         FLOAT NOT NULL,
    [CreationDate] DATETIME NOT NULL, 
    [PersonTaskID] INT NOT NULL IDENTITY, 
    CONSTRAINT [FK_PersonTask_ToPerson] FOREIGN KEY ([PersonID]) REFERENCES [dbo].[Person] ([PersonID]),
    CONSTRAINT [FK_PersonTask_ToTask] FOREIGN KEY ([TaskID]) REFERENCES [dbo].[Task] ([TaskID]) ON DELETE CASCADE, 
    CONSTRAINT [PK_PersonTask] PRIMARY KEY ([PersonTaskID])
);

