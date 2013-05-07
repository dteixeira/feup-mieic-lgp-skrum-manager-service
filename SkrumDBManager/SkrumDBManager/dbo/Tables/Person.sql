CREATE TABLE [dbo].[Person] (
    [PersonID] INT         IDENTITY (1, 1) NOT NULL,
    [Name]     NVARCHAR(128)  NOT NULL,
    [PhotoURL]     NVARCHAR(256) NULL,
    [Email]    NVARCHAR(128) NOT NULL,
    [Admin]    BIT         NOT NULL,
    [JobDescription]      NTEXT  NULL,
    [Password] VARCHAR(128) NULL, 
    PRIMARY KEY CLUSTERED ([PersonID] ASC), 
    CONSTRAINT [AK_Person_Email] UNIQUE ([Email])
);

