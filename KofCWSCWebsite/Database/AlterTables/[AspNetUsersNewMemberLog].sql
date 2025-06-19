CREATE TABLE [dbo].[AspNetUsersNewMemberLog]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [UserID] UNIQUEIDENTIFIER NOT NULL, 
    [ChangeDate] DATETIME NOT NULL, 
    [ChangedBy] NVARCHAR(50) NULL, 
    [PreviousValue] BIT NOT NULL, 
    [NewValue] BIT NOT NULL
)
