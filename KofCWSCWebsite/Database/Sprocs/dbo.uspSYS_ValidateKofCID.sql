ALTER PROCEDURE uspSYS_ValidateKofCID
	@KofCID varchar(7),
	@RetVal int output
AS
--*************************************************************************************************************************
-- 6/22/2024 Tim Philomeno
-- This is used in the Register Validation to make sure that the registration is for a member that we already have
-- on file.  We have to do the filtering on the client becauuse KofCID is an encrypted column
-- uspSYS_ValidateKofCID '1111'
-- select * from tbl_MasMembers where kofcid=0 lastname = 'philomeno'
-- 6/3/2025 Tim Philomeno
-- to support the new register process, i need to make this sproc more intellegent.  This is onlly used
-- when registering a new member
-- Possibles outcomes. Found in tbl_MasMembers
--		In				Is			In			
--tbl_MasMembers	Suspended	AspNetUsers			
--		N				n/a			n/a		No Member	You can register but will need more information	Yes Reg w/Add Info
--		Y				N			N		Is Member	Not Reg	You can register	Yes Reg
--		Y				N			Y		Is Member	Is Registered	You are already registered	Stop Reg
--		n/a				Y			n/a		Member Sus	You are suspended	Stop Reg
--		n/a				Y			n/a		Member Sus	You are suspended	Stop Reg
--	-1 = Member Number is invalid
--	 1 = Member NOT in our db go register with addl info - REGWADDLINFO
--	 2 = is in our data but no profile - ALLOWREG
--	 3 = Member is Suspended - SUS
--	 4 = already a member and profile - REG
/* 
 declare @RetVal int
 exec uspSYS_ValidateKofCID 3970136 ,@RetVal output
 Select @RetVal
 */
 -- 3513961
 -- 1234567 = 4
 -- 111 = -1
 -- 1972699 = 2
 -- 3970136 = 3
--*************************************************************************************************************************
BEGIN
--DECLARE @RetVal int
--SET @RetVal = 0
DECLARE @issus bit,@ismem bit,@ispro bit,@isvalid bit
SET @ismem = 0
SET @issus = 0
SET @ispro = 0
SET @isvalid = 0
SELECT @isvalid = dbo.funSYS_IsKofCIDValid(@KofCID)
IF (@isvalid = 0)
BEGIN
	SET @RetVal = -1
END
ELSE
BEGIN
	IF EXISTS (SELECT * FROM tblSYS_MasMemberSuspension WHERE KofCId = @KofCID) SET @issus=1
	IF EXISTS (SELECT * FROM tbl_MasMembers WHERE KofCID = @KofCID) SET @ismem = 1
	IF EXISTS (SELECT * FROM AspNetUsers WHERE KofCMemberID = 3970136@KofCID) SET @ispro = 1

	IF (@ismem = 0) SET @RetVal = 1 -- Member NOT in our db go register with addl info - REGWADDLINFO
	IF (@ismem = 1 AND @ispro = 0 AND @issus = 0 ) SET @RetVal = 2 -- is in our data but no profile - REG
	IF (@issus = 1 ) SET @RetVal = 3 -- member is suspended - SUS
	IF (@ismem = 1 AND @ispro = 1 AND @issus = 0 ) SET @RetVal = 4 -- already a member and profile - ALREG


	RETURN @RetVal
END
END