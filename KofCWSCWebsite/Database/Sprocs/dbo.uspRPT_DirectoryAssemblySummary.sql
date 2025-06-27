alter PROCEDURE [dbo].[uspRPT_DirectoryAssemblySummary]
@ShortForm INT=0, @NextYear INT=0
AS
SELECT   'FOURTH DEGREE ASSEMBLY SUMMARY' AS GroupName,
         '' AS ReportTitle,
         '' AS ReportSubTitle,
         @ShortForm AS ShortForm,
         @NextYear AS NextYear,
         '' AS FullName,
         '' AS Phone,
         '' AS Address,
         VA.A_LOCATION AS City,
         '' AS State,
         '' AS PostalCode,
         '' AS Email,
         '' AS Title,
         VA.A_NUMBER AS DirSortOrder,
         VA.A_NAME + ' (' + VA.A_LOCATION + ')' AS EntityName,
         '' AS PrintEntity,
         '' AS Meetings
FROM     tbl_ValAssys AS VA
WHERE    VA.A_NUMBER > 0
ORDER BY va.A_NUMBER;