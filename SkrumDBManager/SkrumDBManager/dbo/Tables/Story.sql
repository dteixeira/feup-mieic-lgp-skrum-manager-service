CREATE TABLE [dbo].[Story] (
    [StoryID]   INT        IDENTITY (1, 1) NOT NULL,
    [CreationDate]      DATETIME NOT NULL,
    [NextStory]     INT        NULL,
    [State]     INT NOT NULL,
    [ProjectID] INT        NOT NULL,
    [Description] NTEXT NULL, 
    PRIMARY KEY CLUSTERED ([StoryID] ASC),
    CONSTRAINT [FK_Story_ToProject] FOREIGN KEY ([ProjectID]) REFERENCES [dbo].[Project] ([ProjectID]), 
    CONSTRAINT [FK_Story_ToStoryState] FOREIGN KEY ([State]) REFERENCES [dbo].[StoryState] ([StoryStateID]), 
    CONSTRAINT [FK_Story_ToStory] FOREIGN KEY ([NextStory]) REFERENCES [dbo].[Story] ([StoryID]) 
);

