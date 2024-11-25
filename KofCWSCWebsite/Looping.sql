DECLARE @LoopCounter INT , @MaxID INT, 
        @ID NVARCHAR(100), @MemberID int

SELECT @LoopCounter = min(id) , @MaxID = max(Id) 
FROM tblCVN_ImpDelegates
 
WHILE ( @LoopCounter IS NOT NULL
        AND  @LoopCounter <= @MaxID)
BEGIN
   --SELECT @ID = ID,@MemberID = D1MemberID FROM tblCVN_ImpDelegates 
   SELECT @ID = ID FROM tblCVN_ImpDelegates 
   WHERE Id = @LoopCounter
   ----------------------------------------------------------------------------------------------------------------------------
   -- Begin Do Stuff
   PRINT @ID  
   -- Validate MemberID
   --if Exists(select * from tbl_masmembers where KofCID = @MemberID)
   --   PRINT 'processing member ' + cast(@MemberID as varchar)
	  --else
	  --print 'member not found'
   
   -- End Do Stuff
   ----------------------------------------------------------------------------------------------------------------------------
   -- get the next one
   SELECT @LoopCounter  = min(id) FROM tblCVN_ImpDelegates
   WHERE Id > @LoopCounter
END