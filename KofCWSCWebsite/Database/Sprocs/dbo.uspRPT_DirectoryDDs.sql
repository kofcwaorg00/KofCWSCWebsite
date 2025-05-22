ALTER PROCEDURE [dbo].[uspRPT_DirectoryDDs]
@ShortForm INT=0, @NextYear INT=0
AS
SELECT CASE dd.District WHEN 0 THEN 'District Deputy Director' ELSE 'District Deputies' END AS GroupName,
       '' AS ReportTitle,
       '' AS ReportSubTitle,
       @ShortForm AS ShortForm,
       @NextYear AS NextYear,
       ISNULL(dbo.funSYS_BuildName(MM.MemberID, 0, NULL) + dbo.funSYS_AddWife(MM.MemberID),dd.FullName) AS FullName,
       MM.Phone,
       MM.Address,
       MM.City,
       MM.State,
       MM.PostalCode,
       'DD' + CAST (DD.District AS VARCHAR) + '@KOFC-WA.ORG' AS Email,
       CASE dd.District WHEN 0 THEN 'All Districts' ELSE 'District No. - ' + CAST (DD.DISTRICT AS VARCHAR) END AS Title,
       DD.DISTRICT AS DirSortOrder,
       '' AS EntityName,
       '' AS PrintEntity,
       '' AS Meetings
FROM vewSYS_DDs dd
	LEFT OUTER JOIN tbl_MasMembers mm on dd.MemberID=mm.MemberID
WHERE  DD.Year = dbo.funSYS_GetBegFratYearN(@NextYear)
       AND isnull(MM.Deceased, 0) <> 1