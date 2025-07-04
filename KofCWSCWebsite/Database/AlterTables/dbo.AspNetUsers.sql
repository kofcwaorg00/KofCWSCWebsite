CREATE TABLE [dbo].[AspNetUsers] (
    [Id]                   NVARCHAR (450)     NOT NULL,
    [UserName]             NVARCHAR (256)     NULL,
    [NormalizedUserName]   NVARCHAR (256)     NULL,
    [Email]                NVARCHAR (256)     NULL,
    [NormalizedEmail]      NVARCHAR (256)     NULL,
    [EmailConfirmed]       BIT                NOT NULL,
    [PasswordHash]         NVARCHAR (MAX)     NULL,
    [SecurityStamp]        NVARCHAR (MAX)     NULL,
    [ConcurrencyStamp]     NVARCHAR (MAX)     NULL,
    [PhoneNumber]          NVARCHAR (MAX)     NULL,
    [PhoneNumberConfirmed] BIT                NOT NULL,
    [TwoFactorEnabled]     BIT                NOT NULL,
    [LockoutEnd]           DATETIMEOFFSET (7) NULL,
    [LockoutEnabled]       BIT                NOT NULL,
    [AccessFailedCount]    INT                NOT NULL,
    [FirstName]            NVARCHAR (100)     DEFAULT (N'') NOT NULL,
    [KofCMemberID]         INT                DEFAULT (N'') NOT NULL,
    [LastName]             NVARCHAR (100)     DEFAULT (N'') NOT NULL,
    [ProfilePictureUrl]    NVARCHAR (250)     NULL,
    [Address] NVARCHAR(255) NULL, 
    [City] NVARCHAR(50) NULL, 
    [State] NVARCHAR(10) NULL, 
    [PostalCode] NVARCHAR(20) NULL, 
    [Wife] NVARCHAR(12) NULL, 
    [Council] INT NULL, 
    CONSTRAINT [PK_AspNetUsers] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_AspNetUsers_tbl_MasMembers] FOREIGN KEY ([KofCMemberID]) REFERENCES [dbo].[tbl_MasMembers] ([KofCID])
);


GO
CREATE NONCLUSTERED INDEX [EmailIndex]
    ON [dbo].[AspNetUsers]([NormalizedEmail] ASC);


GO
CREATE UNIQUE NONCLUSTERED INDEX [NonClusteredIndex-KofCMemeberID]
    ON [dbo].[AspNetUsers]([KofCMemberID] ASC);


GO
CREATE UNIQUE NONCLUSTERED INDEX [UserNameIndex]
    ON [dbo].[AspNetUsers]([NormalizedUserName] ASC) WHERE ([NormalizedUserName] IS NOT NULL);


GO
CREATE TRIGGER [dbo].[trgUsersAfterChangeCMO]
    ON [dbo].[AspNetUsers]
    AFTER DELETE, INSERT, UPDATE
    AS BEGIN
           DECLARE @GroupID AS INT;
           SET @GroupID = 0;
           BEGIN TRY
               EXECUTE uspSYS_SyncIdentityRolesWithOfficeGroups @GroupID;
           END TRY
           BEGIN CATCH
               INSERT  INTO ErrorLog (ErrorMessage, ErrorProcedure, ErrorLine)
               VALUES               (ERROR_MESSAGE(), ERROR_PROCEDURE(), ERROR_LINE());
               THROW;
           END CATCH
       END