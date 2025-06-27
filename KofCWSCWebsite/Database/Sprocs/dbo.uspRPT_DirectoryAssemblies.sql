alter PROCEDURE [dbo].[uspRPT_DirectoryAssemblies]
@ShortForm INT=0, @NextYear INT=0
AS
SELECT 'FOURTH DEGREE ASSEMBLIES' AS GroupName,
       '' AS ReportTitle,
       '' AS ReportSubTitle,
       @ShortForm AS ShortForm,
       @NextYear AS NextYear,
       dbo.funSYS_BuildName(MM.MemberID, 0, NULL) + dbo.funSYS_AddWife(MM.MemberID) AS FullName,
       MM.Phone,
       MM.Address,
       MM.City,
       MM.State,
       MM.PostalCode,
       MM.Email,
       VO.OfficeDescription AS Title,
       VA.A_NUMBER AS DirSortOrder,
       VA.A_NAME AS EntityName,
       VA.A_LOCATION + ' - ' + A_NAME + ' - ' + CAST (VA.A_NUMBER AS VARCHAR) AS PrintEntity,
       VA.[ADD INFO 1] + ' ' + VA.[ADD INFO 2] AS Meetings
FROM   tbl_MasMembers AS MM
       INNER JOIN
       tbl_CorrMemberOffice AS MO
       ON MM.MemberID = MO.MemberID
       INNER JOIN
       tbl_ValOffices AS VO
       ON VO.OfficeID = MO.OfficeID
       INNER JOIN
       tbl_ValAssys AS VA
       ON VA.A_NUMBER = CASE mo.OfficeID WHEN 108 THEN mo.Assembly ELSE MM.Assembly END
WHERE  MO.OfficeID IN (17, 20, 108)
       AND mo.Year = dbo.funSYS_GetBegFratYearN(@NextYear)
       AND isnull(MM.Deceased, 0) <> 1
       AND VA.Status = 'A'