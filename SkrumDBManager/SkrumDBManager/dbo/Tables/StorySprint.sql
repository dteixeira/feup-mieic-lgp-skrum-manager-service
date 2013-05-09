CREATE TABLE [dbo].[StorySprint] (
    [Priority]        INT NOT NULL,
    [Points]          INT        NOT NULL,
    [StoryID]         INT        NOT NULL,
    [SprintID]        INT        NOT NULL,
    CONSTRAINT [FK_StorySprint_ToSprint] FOREIGN KEY ([SprintID]) REFERENCES [dbo].[Sprint] ([SprintID]),
    CONSTRAINT [FK_StorySprint_ToStory] FOREIGN KEY ([StoryID]) REFERENCES [dbo].[Story] ([StoryID]), 
    CONSTRAINT [FK_StorySprint_ToStoryPriority] FOREIGN KEY ([Priority]) REFERENCES [dbo].[StoryPriority] ([StoryPriorityID]), 
    CONSTRAINT [PK_StorySprint] PRIMARY KEY ([StoryID], [SprintID])
);

