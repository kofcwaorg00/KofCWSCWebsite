alter PROCEDURE [dbo].[uspRPT_DirectoryCouncilSummary]
@ShortForm INT=0, @NextYear INT=0
AS
SELECT   'SUBORDINATE COUNCIL SUMMARY' AS GroupName,
         '' AS ReportTitle,
         '' AS ReportSubTitle,
         @ShortForm AS ShortForm,
         @NextYear AS NextYear,
         '' AS FullName,
         '' AS Phone,
         '' AS Address,
         vc.c_location AS City,
         '' AS State,
         '' AS PostalCode,
         '' AS Email,
         '' AS Title,
         VC.C_NUMBER AS DirSortOrder,
         VC.C_NAME + ' (' + CAST (VC.DISTRICT AS VARCHAR) + ')' AS EntityName,
         '' AS PrintEntity,
         '' AS Meetings
FROM     tbl_ValCouncils AS VC
WHERE    VC.C_NUMBER > 0
         AND VC.Status = 'A'
ORDER BY vc.C_NUMBER;