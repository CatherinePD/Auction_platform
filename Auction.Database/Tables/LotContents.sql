CREATE TABLE [dbo].[LotContents]
(
    [Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [Content] VARBINARY(MAX) NOT NULL,
    [LotId] INT NOT NULL, 
    CONSTRAINT [FK_LotContents_Lots] FOREIGN KEY ([LotId]) REFERENCES [Lots]([Id])
)
GO

CREATE NONCLUSTERED INDEX IX_Content_LotId
    ON LotContents (LotId);
