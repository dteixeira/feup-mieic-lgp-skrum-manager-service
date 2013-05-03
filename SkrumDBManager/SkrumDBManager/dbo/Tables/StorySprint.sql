CREATE TABLE [dbo].[StorySprint] (
    [StorySprintID] INT        IDENTITY (1, 1) NOT NULL,
    [Priority]        INT NOT NULL,
    [Points]          INT        NOT NULL,
    [StoryID]         INT        NOT NULL,
    [SprintID]        INT        NOT NULL,
    PRIMARY KEY CLUSTERED ([StorySprintID] ASC),
    CONSTRAINT [FK_StorySprint_ToSprint] FOREIGN KEY ([SprintID]) REFERENCES [dbo].[Sprint] ([SprintID]),
    CONSTRAINT [FK_StorySprint_ToStory] FOREIGN KEY ([StoryID]) REFERENCES [dbo].[Story] ([StoryID]), 
    CONSTRAINT [FK_StorySprint_ToStoryPriority] FOREIGN KEY ([Priority]) REFERENCES [dbo].[StoryPriority] ([StoryPriorityID])
);

