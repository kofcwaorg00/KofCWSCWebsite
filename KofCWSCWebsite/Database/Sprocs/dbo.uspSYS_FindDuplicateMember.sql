ALTER PROCEDURE uspSYS_FindDuplicateMembers

AS

BEGIN

	select Firstname,Lastname,Address,MI,count(*) as Number from tbl_MasMembers
	where address is not null
	group by firstname,LastName,Address,MI
	having count(*) > 1

END