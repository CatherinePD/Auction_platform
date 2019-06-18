CREATE TABLE [dbo].[Categories]
(
    [Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [Name] NVARCHAR(75) NOT NULL,
    [Description] NVARCHAR(MAX) NULL 
)
