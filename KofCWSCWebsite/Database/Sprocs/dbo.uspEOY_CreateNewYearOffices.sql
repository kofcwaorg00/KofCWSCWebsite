ALTER PROCEDURE [dbo].[uspEOY_CreateNewYearOffices]
AS
---------------------------------------------------------------------------
-- 5/15/2025 Tim Philomeno
-- this process was modified to only insert the next years offices if
-- they haven't already been added.
-- NOTE: This is the first time I have added a stored procedure to my projce
-- and thus it will get updated and saved to GITHUB CTRL-SHIFT-E will exec
-- select * from tbl_CorrMemberOffice
---------------------------------------------------------------------------
IF NOT EXISTS (SELECT *
               FROM   tbl_CorrMemberOffice AS cmo
                      INNER JOIN
                      tbl_ValOffices AS vo
                      ON cmo.OfficeID = vo.OfficeID
               WHERE  vo.OfficeID IN (SELECT OfficeID
                                      FROM   tbl_ValOffices
                                      WHERE  Copy2NewYear = 1)
                      AND cmo.Year = dbo.funSYS_GetBegFratYearN(1))
    BEGIN
        INSERT tbl_CorrMemberOffice
        SELECT cmo.MemberID,
               cmo.OfficeID,
               cmo.PrimaryOffice,
               dbo.funSYS_GetBegFratYearN(1) AS [Year],
               cmo.District,
               cmo.Council,
               cmo.Assembly,
               GETDATE(),
               9999999
        FROM   tbl_CorrMemberOffice AS cmo
               INNER JOIN
               tbl_ValOffices AS vo
               ON cmo.OfficeID = vo.OfficeID
        WHERE  vo.OfficeID IN (SELECT OfficeID
                               FROM   tbl_ValOffices
                               WHERE  Copy2NewYear = 1)
               AND cmo.Year = dbo.funSYS_GetBegFratYearN(0);
    END
ELSE
    BEGIN
        PRINT 'Next Year has already been PRIMED';
        RETURN -1;
    END