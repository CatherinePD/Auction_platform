CREATE TABLE [dbo].[Bids]
(
    [Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [Amount] MONEY NOT NULL,
    [LotId] INT NOT NULL,
    [DateCreated] DATETIME NOT NULL,
    [UserId] INT NOT NULL,
    CONSTRAINT [FK_Bids_Users] FOREIGN KEY ([UserId]) REFERENCES [Users]([Id]),
    CONSTRAINT [FK_Bids_Lots] FOREIGN KEY ([LotId]) REFERENCES [Lots]([Id])
)
GO

CREATE NONCLUSTERED INDEX IX_Bids_UserId
    ON Bids (UserId);
GO
CREATE NONCLUSTERED INDEX IX_Bids_Lots
    ON Bids (LotId);
