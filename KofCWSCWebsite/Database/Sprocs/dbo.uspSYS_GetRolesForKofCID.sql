ALTER PROCEDURE [dbo].[uspSYS_GetRolesForKofCID]
@KofCID INT=0
AS
--------------------------------------------------------------------------
-- 5/15/2025 Tim Philomeno
-- Gets a list of roles for a user
-- NOTE: We are using KofCID because that is the FK to tbl_MasMembers
--------------------------------------------------------------------------
--select * from aspnetusers where kofcmemberid= 4294725
--exec [uspSYS_GetRolesForKofCID] 4294725
--------------------------------------------------------------------------
SELECT u.FirstName+' '+u.LastName as Name,r.Name as Role
FROM   AspNetUserRoles AS ur
       INNER JOIN AspNetUsers AS u ON ur.UserId = u.Id
       INNER JOIN AspNetRoles AS r ON ur.RoleId=r.Id
WHERE  u.KofCMemberID = @KofCID;
