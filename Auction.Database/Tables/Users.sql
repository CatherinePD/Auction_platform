CREATE TABLE [dbo].[Users]
(
    [Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [Login] NVARCHAR(75) NOT NULL,
    [ContactId] INT NULL,
    [Password] NVARCHAR(150) NOT NULL
    CONSTRAINT [FK_Users_Contacts] FOREIGN KEY ([ContactId]) REFERENCES [Contacts]([Id])
)
GO

CREATE NONCLUSTERED INDEX IX_Users_ContacttId
    ON Users (ContactId);
