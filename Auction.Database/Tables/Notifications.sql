CREATE TABLE [dbo].[Notifications]
(
    [Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [Message] NVARCHAR(MAX) NOT NULL,
    [RecieverId] INT NULL,
    [IsReaded] BIT NOT NULL DEFAULT 0,
    [DateCreated] DATETIME NOT NULL,
    [DateReaded] DATETIME NULL,
    [Source] TINYINT NOT NULL,
    [EntityId] INT NULL
    CONSTRAINT [FK_Notifications_Users] FOREIGN KEY ([RecieverId]) REFERENCES [Users]([Id])
)
