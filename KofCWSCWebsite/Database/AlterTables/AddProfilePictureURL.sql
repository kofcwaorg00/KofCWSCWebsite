alter table aspnetusers
--add ProfilePictureUrl nvarchar(250) null,
add    [Address] NVARCHAR(255) NULL, 
    [City] NVARCHAR(50) NULL, 
     [State] NVARCHAR(10) NULL, 
    [PostalCode] NVARCHAR(20) NULL, 
    [Wife] NVARCHAR(12) NULL, 
    [Council] INT NULL

alter table aspnetusers
add    [MemberVerified] bit null

alter table aspnetusers
add MembershipCardUrl nvarchar(250) null

ALTER TABLE [dbo].[AspNetUsers]  
DROP CONSTRAINT [FK_AspNetUsers_tbl_MasMembers]