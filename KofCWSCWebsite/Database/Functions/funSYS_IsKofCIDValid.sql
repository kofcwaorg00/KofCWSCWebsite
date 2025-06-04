alter FUNCTION dbo.funSYS_IsKofCIDValid(@KofCID int)
RETURNS bit
AS
BEGIN
-- 6/4/2025 Tim Philomeno
-- Rules for a valid KofCID. This is used by uspSYS_ValidateKofCID
-- if 0 then 0
-- if < 10000 or > 6000000 then 0

-- select dbo.funSYS_IsKofCIDValid(12332121)
    DECLARE @RetVal bit
    SET @RetVal = 0
    -- this ends up being very simple because we are dealing with an int
    IF(@KofCID >= 10000 AND @KofCID <= 6000000) SET @RetVal = 1
    RETURN @RetVal
END