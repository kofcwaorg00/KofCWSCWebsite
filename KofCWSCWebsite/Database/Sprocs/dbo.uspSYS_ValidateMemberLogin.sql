create PROCEDURE [dbo].[uspSYS_ValidateMemberLogin]
@MemberLogin VARCHAR (255)
AS
BEGIN
-- exec [uspSYS_ValidateMemberLogin] 'tphilomeno@comcast.net'
    SELECT au.KofCMemberID
    FROM   AspNetUsers AS au
           INNER JOIN
           tblSYS_MasMemberSuspension AS ms
           ON au.KofCMemberID = ms.KofCId
    WHERE  au.Email = @MemberLogin;
END