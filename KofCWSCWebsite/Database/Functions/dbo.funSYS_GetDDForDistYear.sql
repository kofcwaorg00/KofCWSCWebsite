CREATE FUNCTION [dbo].[funSYS_GetDDForDistYear]
(
	@District int,
	@Year int
)
RETURNS INT
AS
BEGIN
-- this will return NULL if vacant for the year
-- select dbo.[funSYS_GetDDForDistYear](0,2024)
--select * from tbl_valoffices where OfficeDescription like '%district%'

	DECLARE @RetVal int

	SELECT @RetVal=MemberID
	FROM tbl_CorrMemberOffice
	-- include DDD
	WHERE Year=@Year and OfficeID IN(13,129) and District=@District
	RETURN @RetVal
END
