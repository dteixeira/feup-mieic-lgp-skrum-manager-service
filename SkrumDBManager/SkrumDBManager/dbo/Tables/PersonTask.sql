CREATE TABLE [dbo].[PersonTask] (
    [TaskID]       INT NOT NULL,
    [PersonID]     INT NOT NULL,
    [SpentTime]         FLOAT NOT NULL,
    [CreationDate] DATETIME NOT NULL, 
    PRIMARY KEY CLUSTERED ([TaskID], [PersonID]),
    CONSTRAINT [FK_PersonTask_ToPerson] FOREIGN KEY ([PersonID]) REFERENCES [dbo].[Person] ([PersonID]),
    CONSTRAINT [FK_PersonTask_ToTask] FOREIGN KEY ([TaskID]) REFERENCES [dbo].[Task] ([TaskID])
);

