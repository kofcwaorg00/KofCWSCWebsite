


alter   function [dbo].[funSYS_GetMemberPrimeTitle](@MemberID integer)
	RETURNS varchar(100)
AS
BEGIN
-- SELECT dbo.funGetMemberTitle(17441)
	DECLARE @Office varchar(100),@OfficeID integer,@District integer,@Council integer,
			@Assembly integer
	
	
	
	SELECT @Office=AltDescription,@OfficeID=VO.OfficeID
	FROM tbl_CorrMemberOffice MO
		INNER JOIN tbl_ValOffices VO ON
			MO.OfficeID=VO.OfficeID AND
			MO.Year=dbo.funSYS_GetBegFratYearN(0) AND
			PrimaryOffice=1
	WHERE MemberID=@MemberID
	
	SELECT @District=VC.District,@Council=MM.Council,@Assembly=ISNULL(MM.Assembly,'')
	FROM tbl_MasMembers MM 
		INNER JOIN tbl_ValCouncils VC ON
			MM.Council=VC.C_NUMBER
	WHERE MemberID=@MemberID
	IF @OfficeID=13 -- DD
	BEGIN
		SET @Office = @Office + ' ' + char(35) + convert(varchar,@District)
	END
	ELSE IF @OfficeID=17 -- FC
	BEGIN
		SET @Office = @Office + ' ' + char(35) + convert(varchar,@Assembly)
	END
	ELSE IF @OfficeID=20 -- FN
	BEGIN
		SET @Office = @Office + ' ' + char(35) + convert(varchar,@Assembly)
	END
	ELSE IF @OfficeID=22 -- FS
	BEGIN
		SET @Office = @Office + ' ' + char(35) + convert(varchar,@Council)
	END
	ELSE IF @OfficeID=27 -- GK
	BEGIN
		SET @Office = @Office + ' ' + char(35) + convert(varchar,@Council)
	END
	ELSE IF @Office IS NULL -- didn't find a primary office flag...
	BEGIN
		SELECT @Office='Convention Delegate'
		FROM tbl_CorrMemberOffice MO
			INNER JOIN tbl_ValOffices VO ON
				MO.OfficeID=VO.OfficeID AND
				MO.Year=dbo.funSYS_GetBegFratYearN(0)
		WHERE MemberID=@MemberID AND MO.OfficeID IN(115,116,118,119)
	END
	-- exception for now.....!!!
	IF @MemberID = 879 -- Tim Philomeno
	BEGIN
		SET @Office='Webmaster'
	END
	
	RETURN isnull(' '+@Office,'')
END
/*
select * from tbl_valcouncils
select *
from tbl_valgroups
select * from tbl_valoffices
SELECT *
FROM tbl_MasMembers 
WHERE MemberID in(SELECT MemberID
					FROM tbl_CorrMemberOffice
					WHERE OfficeID IN(13))
*/