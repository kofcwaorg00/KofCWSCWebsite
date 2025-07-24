ALTER PROCEDURE [dbo].[uspSYS_MoveCouncil]
	@FromCouncil int,
	@ToCouncil int,
	@Post bit
	
AS


/*
	To move a council we must select all memebers from the council to be moved and
	change the Council value to the new council number.  Then set the council to be moved
	status to 'I'(nactive)

	[uspSYS_MoveCouncil] 12483,13238,FALSE
	[uspSYS_MoveCouncil] 1,1,FALSE

	12483 no longer exists, it merged with council 13238 

	select * from tbl_MasMembers where council = 10532 --78
	select * from tbl_MasMembers where council = 8455 --88
	select * from tbl_ValCOuncils where c_number = 10532
*/

BEGIN

	IF @FromCouncil is null or @FromCouncil = 0
	BEGIN
		SELECT 'From Council must have a value. No Action Taken' as MessageText
		RETURN
	END

	IF @ToCouncil is null or @ToCouncil = 0
	BEGIN
		SELECT 'From Council must have a value. No Action Taken' as MessageText
		RETURN
	END

	IF NOT EXISTS(SELECT * FROM tbl_MasMembers WHERE Council = @FromCouncil)
	BEGIN
		SELECT 'No Members Exist for Source Council #' + cast(@FromCouncil as varchar)+' No Action Taken, Please Try Again' as MessageText 
		RETURN
	END

	IF NOT EXISTS(SELECT * FROM tbl_MasMembers WHERE Council = @ToCouncil)
	BEGIN
		SELECT 'No Members from Destination Council #' + cast(@FromCouncil as varchar) + ' To Council #' + cast(@ToCouncil as varchar) + 'No Action Taken. Please Try Again' as MessageText
		RETURN
	END

	IF @Post = 'TRUE'
	BEGIN
		DECLARE @NoMembersTrue int
		SET @NoMembersTrue=(select count(*) from tbl_MasMembers where Council=@FromCouncil)
		SELECT 'Moving '+ cast(@NoMembersTrue as varchar) + ' Members from Council #' + cast(@FromCouncil as varchar) + ' to Council #' + cast(@ToCouncil as varchar) as MessageText
		-- If we get here then we should do the deed
		update mm
		SET mm.council=@ToCouncil,mm.CouncilUpdated=getdate(),mm.CouncilUpdatedBy='Move Council SPROC',mm.DataChanged=1,mm.LastUpdated=getdate()
		FROM tbl_MasMembers mm
		WHERE mm.council = @FromCouncil

		UPDATE vc
		SET vc.Status='I'
		FROM tbl_ValCouncils vc
		WHERE vc.C_NUMBER = @FromCouncil
	END
	ELSE
	BEGIN
		DECLARE @NoMembersFalse int
		SET @NoMembersFalse=(select count(*) from tbl_MasMembers where Council=@FromCouncil)
		SELECT 'Setting the POST parameter to TRUE will move '+ cast(@NoMembersFalse as varchar) + ' Members from Council #' + cast(@FromCouncil as varchar) + ' to Council #' + cast(@ToCouncil as varchar) as MessageText
	END

END