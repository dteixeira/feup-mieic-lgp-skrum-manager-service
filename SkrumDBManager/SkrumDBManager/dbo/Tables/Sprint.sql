CREATE TABLE [dbo].[Sprint] (
    [SprintID]   INT      IDENTITY (1, 1) NOT NULL,
    [Number]        INT      NOT NULL,
    [BeginDate] DATETIME NOT NULL,
    [EndDate]    DATETIME NULL,
    [Closed]     BIT      NOT NULL,
    [ProjectID]  INT      NOT NULL,
    PRIMARY KEY CLUSTERED ([SprintID] ASC),
    CONSTRAINT [FK_Sprint_ToProject] FOREIGN KEY ([ProjectID]) REFERENCES [dbo].[Project] ([ProjectID]) ON DELETE CASCADE
);

