CREATE TABLE [dbo].[StorySprint] (
    [Points]          INT        NOT NULL,
    [StoryID]         INT        NOT NULL,
    [SprintID]        INT        NOT NULL,
    CONSTRAINT [FK_StorySprint_ToSprint] FOREIGN KEY ([SprintID]) REFERENCES [dbo].[Sprint] ([SprintID]) ON DELETE CASCADE,
    CONSTRAINT [FK_StorySprint_ToStory] FOREIGN KEY ([StoryID]) REFERENCES [dbo].[Story] ([StoryID]) ON DELETE CASCADE, 
    CONSTRAINT [PK_StorySprint] PRIMARY KEY ([StoryID], [SprintID])
);

