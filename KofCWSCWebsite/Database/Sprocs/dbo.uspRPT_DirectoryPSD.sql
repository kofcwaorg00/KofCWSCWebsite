ALTER PROCEDURE [dbo].[uspRPT_DirectoryPSD]
@ShortForm INT=0, @NextYear INT=0
AS
SELECT   'PAST STATE DEPUTIES' AS GroupName,
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
         VO.DirSortOrder,
         '' AS EntityName,
         '' AS PrintEntity,
         '' AS Meetings
FROM     tbl_MasMembers AS MM
         INNER JOIN
         tbl_CorrMemberOffice AS MO
         ON MM.MemberID = MO.MemberID
         INNER JOIN
         tbl_ValOffices AS VO
         ON VO.OfficeID = MO.OfficeID
         INNER JOIN
         tbl_ValCouncils AS VC
         ON MM.Council = VC.C_NUMBER
         INNER JOIN
         tbl_ValDistricts AS VD
         ON VC.DISTRICT = VD.DISTRICT
WHERE    VO.GroupID = 8
         AND mo.Year = dbo.funSYS_GetBegFratYearN(@NextYear)
         AND isnull(MM.Deceased, 0) <> 1
         AND VO.OfficeDescription LIKE 'PSD%'
ORDER BY vo.DirSortOrder;