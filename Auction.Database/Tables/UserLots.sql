CREATE TABLE [dbo].[UserLots]
(
    [UserId] INT NOT NULL,
    [LotId] INT NOT NULL,
    CONSTRAINT [PK_UserLots] PRIMARY KEY NONCLUSTERED ([UserId], [LotId]),
    CONSTRAINT [FK_UserLots_Users] FOREIGN KEY ([UserId]) REFERENCES [Users]([Id]),
    CONSTRAINT [FK_UserLots_Lots] FOREIGN KEY ([LotId]) REFERENCES [Lots]([Id])
)
GO

CREATE NONCLUSTERED INDEX IX_UserLots_UserId
    ON UserLots (UserId);
GO

CREATE NONCLUSTERED INDEX IX_UserLots_LotId
    ON UserLots (LotId);
GO
