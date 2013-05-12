CREATE TABLE [dbo].[Task] (
    [TaskID]     INT        IDENTITY (1, 1) NOT NULL,
    [CreationDate]       DATETIME NOT NULL,
    [Estimation] INT        NOT NULL,
    [StoryID]    INT        NOT NULL,
    [Description] NTEXT NULL, 
    [State] INT NOT NULL, 
    PRIMARY KEY CLUSTERED ([TaskID] ASC),
    CONSTRAINT [FK_Task_ToStory] FOREIGN KEY ([StoryID]) REFERENCES [dbo].[Story] ([StoryID]) ON DELETE CASCADE, 
    CONSTRAINT [FK_Task_ToTaskState] FOREIGN KEY ([State]) REFERENCES [dbo].[TaskState]([TaskStateID]) ON DELETE CASCADE
);

