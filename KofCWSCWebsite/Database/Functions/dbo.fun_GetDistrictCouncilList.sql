
ALTER FUNCTION [dbo].[fun_GetDistrictCouncilList](@District integer)
	RETURNS varchar(1000)
	
/*
select dbo.fun_GetDistrictCouncilList(0)
*/
BEGIN
	DECLARE @RetVal varchar(1000),@Count integer,@temp varchar(100),@Location varchar(1000),
		@RC INT, @LS INT, @LC INT,
		@RC1 int,@LS1 int,@LC1 integer
		
	SET @RetVal=''
	
	DECLARE @Table table(Loc varchar(1000),ObjID integer IDENTITY(1,1))

	INSERT @Table(Loc)
	SELECT CASE VC.C_LOCATION WHEN 'N/A' THEN 'District Depty Director' ELSE vc.C_LOCATION END as C_LOCATION
	FROM tbl_ValCouncils VC
	WHERE VC.District=@District and Status <> 'I'
	GROUP BY VC.C_LOCATION

	SET @RC1=0
	SET @LS1=1
	
	SELECT @LC1 = MIN(ObjID)
	FROM @Table

	WHILE @LS1=1
	BEGIN
		SELECT @RetVal=@RetVal+Loc+', ',@Location=Loc
		FROM @Table
		WHERE ObjID=@LC1
	
		SET @RC = 0
		SET @LS = 1
		SELECT @LC = MIN(C_NUMBER) 
			FROM tbl_ValCouncils
			WHERE District = @District AND
				c_location=@Location and status <> 'I'

		WHILE(@LS = 1)
		BEGIN
			---------------------------------------------------------------------------------
			-- BEGIN LOOP
			---------------------------------------------------------------------------------
			SELECT @RetVal=@RetVal+cast(C_NUMBER as varchar)+', '
			FROM tbl_ValCouncils V
			WHERE V.C_NUMBER=@LC  and v.Status <> 'I'
			---------------------------------------------------------------------------------
			-- END LOOP
			---------------------------------------------------------------------------------
			SELECT @LC = MIN(C_NUMBER)
			FROM tbl_ValCouncils
			WHERE District = @District AND 
				c_Location = @Location AND C_NUMBER > @LC  and Status <> 'I'

			IF (@LC IS NULL) -- if new ObjID is null we are done 
			BEGIN
				SELECT @LS = 0 -- Set the status to 0 to stop looping 
			END
		END
		
		SELECT @LC1 = MIN(ObjID)
		FROM @Table
		WHERE ObjID > @LC1

		IF (@LC1 IS NULL) -- if new ObjID is null we are done 
		BEGIN
			SELECT @LS1 = 0 -- Set the status to 0 to stop looping 
		END
	END
	
	SET @RetVal=substring(@RetVal,1,len(@RetVal)-1)
	RETURN @RetVal
END