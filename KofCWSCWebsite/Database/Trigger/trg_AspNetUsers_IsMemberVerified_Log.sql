CREATE TRIGGER trg_AspNetUsers_IsMemberVerified_Log
ON AspNetUsers
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO AspNetUsersNewMemberLog (
        UserId,
        ChangeDate,
        ChangedBy,
        PreviousValue,
        NewValue
    )
    SELECT 
        i.Id,
        GETDATE(),
        SYSTEM_USER,           -- Optional: change to a column from your app context if needed
        d.MemberVerified,
        i.MemberVerified
    FROM inserted i
    INNER JOIN deleted d ON i.Id = d.Id
    WHERE i.MemberVerified = 0 AND d.MemberVerified <> 0;
END;
