alter FUNCTION [dbo].[funSYS_GetPhotoURL](@KofCID int = 0)
RETURNS nvarchar(500)
	
AS
BEGIN
/*
	5/29/2025 Tim Philomeno
	We need to be able to handle getting the photo url even if the member has
	not registered in the site yet.
	First: check to see if the member is registered
	IF NO then return a default URL based on wwwroot/images/defaultprofilepics/<memberid>
	IF YES then return the Picture URL from the profile
*/
-- select [dbo].[funSYS_GetPhotoURL](4594382)
-- select * from aspnetusers where kofcmemberid=4870062
	DECLARE @RetVal nvarchar(500)
	IF EXISTS (SELECT * FROM AspNetUsers WHERE KofCMemberID = @KofCID)
	BEGIN
		--SELECT @RetVal=ISNULL(ProfilePictureUrl,'/images/defaultprofilepics/'+cast(@KofCID as nvarchar(10))+'.png') FROM AspNetUsers WHERE KofCMemberID = @KofCID
		SELECT @RetVal=ProfilePictureUrl FROM AspNetUsers WHERE KofCMemberID = @KofCID
	END
	ELSE
	BEGIN
		SELECT @RetVal='/images/missingA.png'
	END

	RETURN @RetVal
END