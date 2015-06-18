CREATE TABLE [dbo].[User] (
    [Id]          INT           IDENTITY (1, 1) NOT NULL,
    [Email]       NVARCHAR (56) NOT NULL,
    [IsSuspended] BIT           DEFAULT ((0)) NOT NULL,
    [Avatar_Path] VARCHAR (MAX) NULL,
    [Name]        VARCHAR (56)  NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    UNIQUE NONCLUSTERED ([Email] ASC)
);

