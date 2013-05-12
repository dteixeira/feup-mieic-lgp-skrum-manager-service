CREATE TABLE [dbo].[Meeting] (
    [MeetingID] INT         IDENTITY (1, 1) NOT NULL,
    [Date]      DATETIME    NOT NULL,
    [Number]       INT         NOT NULL,
    [Notes]     NTEXT NULL,
    [ProjectID] INT         NOT NULL,
    PRIMARY KEY CLUSTERED ([MeetingID] ASC),
    CONSTRAINT [FK_Meeting_ToProject] FOREIGN KEY ([ProjectID]) REFERENCES [dbo].[Project] ([ProjectID]) ON DELETE CASCADE
);

