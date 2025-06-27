alter PROCEDURE [dbo].[uspRPT_DirectoryCouncils]
@ShortForm INT=0, @NextYear INT=0
AS
SELECT 'SUBORDINATE COUNCILS' AS GroupName,
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
       VC.C_NUMBER AS DirSortOrder,
       VC.C_NAME AS EntityName,
       CAST (VC.C_NUMBER AS VARCHAR) + ' ' + VC.C_LOCATION + ' - ' + vc.C_NAME + '(' + CAST (vc.DISTRICT AS VARCHAR) + ')' AS PrintEntity,
       VC.[ADD INFO 1] + ' ' + VC.[ADD INFO 2] AS Meetings
FROM   tbl_MasMembers AS MM
       INNER JOIN
       tbl_CorrMemberOffice AS MO
       ON MM.MemberID = MO.MemberID
       INNER JOIN
       tbl_ValOffices AS VO
       ON VO.OfficeID = MO.OfficeID
       INNER JOIN
       tbl_ValCouncils AS VC
       ON VC.C_NUMBER = CASE mo.OfficeID WHEN 167 THEN mo.council ELSE mm.Council END
       INNER JOIN
       tbl_ValDistricts AS VD
       ON VC.DISTRICT = VD.DISTRICT
WHERE  MO.OfficeID IN (27, 22, 167)
       AND isnull(MM.Deceased, 0) <> 1
       AND mo.Year = dbo.funSYS_GetBegFratYearN(@NextYear)
       AND VC.Status = 'A';