CREATE TABLE [dbo].[Task] (
    [TaskID]     INT        IDENTITY (1, 1) NOT NULL,
    [CreationDate]       DATETIME NOT NULL,
    [Estimation] INT        NOT NULL,
    [StoryID]    INT        NOT NULL,
    [PersonID]   INT        NULL,
    [Description] NTEXT NULL, 
    PRIMARY KEY CLUSTERED ([TaskID] ASC),
    CONSTRAINT [FK_Task_ToPerson] FOREIGN KEY ([PersonID]) REFERENCES [dbo].[Person] ([PersonID]),
    CONSTRAINT [FK_Task_ToStory] FOREIGN KEY ([StoryID]) REFERENCES [dbo].[Story] ([StoryID])
);

