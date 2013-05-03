CREATE TABLE [dbo].[Project] (
    [ProjectID] INT        IDENTITY (1, 1) NOT NULL,
    [Password]  VARCHAR(128) NULL,
    [SprintDuration] INT        NOT NULL,
    [AlertLimit]  INT        NOT NULL,
    [Speed]  INT        NOT NULL,
    PRIMARY KEY CLUSTERED ([ProjectID] ASC)
);

