alter PROCEDURE uspLOG_UpdateProcessFlag
@ID INT
AS
BEGIN
--select * from tblLOG_CorrMemberOffice
    IF(@ID = 0) -- if we pass in 0 do them all
    BEGIN
        UPDATE tblLOG_CorrMemberOffice
    SET    Processed = 1
    END
    ELSE
    BEGIN
        UPDATE tblLOG_CorrMemberOffice
    SET    Processed = 1
    WHERE  ID = @ID;
    END

END