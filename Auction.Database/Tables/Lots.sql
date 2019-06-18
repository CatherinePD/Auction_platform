CREATE TABLE [dbo].[Lots]
(
    [Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [Title] NVARCHAR(75) NOT NULL,
    [Description] NVARCHAR(MAX) NULL,
    [DateCreated] DATETIME NOT NULL,
    [DateToExpire] DATETIME NOT NULL,
    [DateFinished] DATETIME NULL,
    [StartBid] MONEY NOT NULL,
    [CurrentBid] MONEY NOT NULL,
    [CategoryId] INT NOT NULL,
    [OwnerId] INT NOT NULL, 
    [IsActive] BIT NOT NULL DEFAULT(1),
    CONSTRAINT [FK_Lots_Users] FOREIGN KEY ([OwnerId]) REFERENCES [Users]([Id]),
    CONSTRAINT [FK_Lots_Categories] FOREIGN KEY ([CategoryId]) REFERENCES [Categories](Id)
)
GO

CREATE NONCLUSTERED INDEX IX_Lots_UserId
    ON Lots (OwnerId);
GO
CREATE NONCLUSTERED INDEX IX_Lots_CategoryId
    ON Lots (CategoryId);
GO
