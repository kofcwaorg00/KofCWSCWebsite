--CREATE PROCEDURE uspSYS_GetContactForCouncil

--AS
-- this will always return 1 record and it migh be all nulls
BEGIN
	DECLARE @Type nchar(2),@Council int
	SET @Type='FS'
	SET @Council=488

	IF @Type = 'GK'
	BEGIN
		IF EXISTS (SELECT * FROM tbl_ValCouncils vc
			LEFT OUTER JOIN vewSYS_GKs GK ON GK.CouncilNo = vc.C_NUMBER and gk.Year = dbo.funSYS_GetBegFratYearN(0)
			WHERE vc.C_NUMBER > 0 and VC.C_NUMBER=@Council)
		BEGIN
			SELECT 'GK' as Office,GK.CouncilNo,GK.GrandKnight as Officer,GK.GKEmail as Email,vc.DISTRICT as District 
			FROM tbl_ValCouncils vc
			LEFT OUTER JOIN vewSYS_GKs GK ON GK.CouncilNo = vc.C_NUMBER and gk.Year = dbo.funSYS_GetBegFratYearN(0)
			WHERE vc.C_NUMBER > 0 and VC.C_NUMBER=@Council
		END
		ELSE
		BEGIN
			SELECT @Type as Office,NULL as CouncilNo,NULL as Officer,NULL as Email,0 AS District
		END
	END
	--UNION ALL
	ELSE
	BEGIN
		IF EXISTS (SELECT * FROM tbl_ValCouncils vc
		LEFT OUTER JOIN vewSYS_FSs FS ON FS.CouncilNo = vc.C_NUMBER and FS.Year = dbo.funSYS_GetBegFratYearN(0)
		WHERE vc.C_NUMBER > 0 and vc.C_NUMBER = @Council)
		BEGIN
			SELECT 'FS' as Office,FS.CouncilNo,FS.FinSec as Officer,FS.FSEmail as Email,vc.DISTRICT as District 
			FROM tbl_ValCouncils vc
			LEFT OUTER JOIN vewSYS_FSs FS ON FS.CouncilNo = vc.C_NUMBER and FS.Year = dbo.funSYS_GetBegFratYearN(0)
			WHERE vc.C_NUMBER > 0 and vc.C_NUMBER = @Council
		END
		ELSE 
		BEGIN
			SELECT @Type as Office,NULL as CouncilNo,NULL as Officer,NULL as Email,0 AS District
		END
	END
END