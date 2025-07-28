create PROCEDURE uspSYS_FindDuplicateMembers

AS

BEGIN

	select Firstname,Lastname,Address,count(*) as Number from tbl_MasMembers
	where address is not null
	group by firstname,LastName,Address
	having count(*) > 1

END