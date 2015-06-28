CREATE TABLE [dbo].[Report] (
    [Id]             INT          IDENTITY (1, 1) NOT NULL,
    [TimeStampStart] VARCHAR (80) NOT NULL,
    [TimeStampStop]  VARCHAR (80) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO
CREATE TABLE [dbo].[Error] (
    [Id]             INT           IDENTITY (1, 1) NOT NULL,
    [ReportId]       INT           NULL,
    [WebPage]        VARCHAR (MAX) NULL,
    [Link]           VARCHAR (MAX) NULL,
    [ErrorStatus]    VARCHAR (MAX)  NULL,
    [Depth]          INT           NULL,
    [ErrorTimeStamp] VARCHAR (80)  NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Report] FOREIGN KEY ([ReportId]) REFERENCES [dbo].[Report] ([Id])
);

