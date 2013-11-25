CREATE TABLE [dbo].[Story] (
    [StoryID]   INT        IDENTITY (1, 1) NOT NULL,
    [CreationDate]      DATETIME NOT NULL,
    [PreviousStory]     INT        NULL DEFAULT null,
    [State]     INT NOT NULL,
    [ProjectID] INT        NULL,
    [Description] NTEXT NULL, 
    [Number] INT NOT NULL, 
    [Priority] INT NOT NULL, 
    PRIMARY KEY CLUSTERED ([StoryID] ASC),
    CONSTRAINT [FK_Story_ToProject] FOREIGN KEY ([ProjectID]) REFERENCES [dbo].[Project] ([ProjectID]) ON DELETE SET NULL, 
    CONSTRAINT [FK_Story_ToStoryState] FOREIGN KEY ([State]) REFERENCES [dbo].[StoryState] ([StoryStateID]) ON DELETE CASCADE, 
    CONSTRAINT [FK_Story_ToStory] FOREIGN KEY ([PreviousStory]) REFERENCES [dbo].[Story] ([StoryID]) ON DELETE NO ACTION, 
    CONSTRAINT [FK_Story_ToStoryPriority] FOREIGN KEY ([Priority]) REFERENCES [dbo].[StoryPriority]([StoryPriorityID]) ON DELETE CASCADE 
);

