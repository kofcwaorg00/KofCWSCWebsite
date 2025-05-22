CREATE FUNCTION dbo.funSYS_GetDDYears()
RETURNS @RetVal TABLE (District INT, Year INT)
AS
BEGIN
-- select * from dbo.funSYS_GetDDYears()
    DECLARE @MinYear INT, @MaxYear INT;

    -- Get Min and Max Year from tbl_CorrMemberOffice
    SELECT @MinYear = MIN([YEAR]), @MaxYear = MAX([YEAR]) FROM tbl_CorrMemberOffice WHERE OfficeID = 13;

    -- Insert District and Year combinations into @RetVal
    INSERT INTO @RetVal (District, Year)
    SELECT d.District, y.YearNum
    FROM tbl_ValDistricts d
    CROSS JOIN (
        SELECT @MinYear + n AS YearNum
        FROM (SELECT TOP (1 + @MaxYear - @MinYear) ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) - 1 AS n FROM master.dbo.spt_values) t
    ) y
    --WHERE d.District > 0;

    RETURN;
END;