alter PROCEDURE [dbo].[uspWEB_GetSOSView]
@NextYear INT=0
AS
-- select * from AspNetUsers

--https://kofcwscdatastorageblob.blob.core.windows.net/profilepics/b86e21ba-b6f2-4772-bcc2-b8ac6b1d0a71.png

SELECT   'State Officers ' + CAST (dbo.funSYS_GetBegFratYearN(@NextYear) AS VARCHAR) + ' - ' + CAST (dbo.funSYS_GetBegFratYearN(@NextYear) + 1 AS VARCHAR) AS Heading,
         au.ProfilePIctureUrl as Photo,
         --'/images/SOs/' + CAST (dbo.funSYS_GetBegFratYearN(@NextYear) AS VARCHAR) + '/' + Replace(vo.OfficeDescription, ' ', '') + '.png' AS Photo,
         vo.EmailAlias + '@kofc-wa.org' AS Email,
         isnull(dbo.funSYS_BuildName(mm.MemberID, 0, ''), '') AS FullName,
         vo.OfficeDescription,
         '/ContactUs?messageRecipient=State%20Officers: ' + vo.OfficeDescription AS ContactUs,
         isnull(mm.MemberID, 0) AS MemberID,
         'TblMasMembers/Details/' AS DirInfo,
         CASE vo.OfficeID WHEN 46 THEN 1 WHEN 47 THEN 3 WHEN 49 THEN 4 WHEN 50 THEN 5 WHEN 45 THEN 6 WHEN 51 THEN 7 WHEN 30 THEN 8 ELSE 0 END AS SortBy
FROM     tbl_ValOffices AS vo 
        LEFT OUTER JOIN tbl_CorrMemberOffice AS cmo ON vo.OfficeID = cmo.OfficeID AND cmo.Year = dbo.funSYS_GetBegFratYearN(@NextYear)
        LEFT OUTER JOIN tbl_MasMembers AS mm ON cmo.MemberID = mm.MemberID
        LEFT OUTER JOIN AspNetUsers au on mm.KofCID=au.KofCMemberID
        WHERE    GroupID = 2
UNION ALL
SELECT   '' AS Heading,
         '' AS Photo,
         '' AS Email,
         '' AS FullName,
         '' AS OfficeDescription,
         '' AS ContactUs,
         0 AS MemberID,
         '' AS DirInfo,
         2 AS SortBy
UNION ALL
SELECT   'State Officers ' + CAST (dbo.funSYS_GetBegFratYearN(@NextYear) AS VARCHAR) + ' - ' + CAST (dbo.funSYS_GetBegFratYearN(@NextYear) + 1 AS VARCHAR) AS Heading,
         --'/images/SOs/' + CAST (dbo.funSYS_GetBegFratYearN(@NextYear) AS VARCHAR) + '/' + Replace(vo.OfficeDescription, ' ', '') + '.png' AS Photo,
         au.ProfilePIctureUrl as Photo,
         vo.EmailAlias + '@kofc-wa.org' AS Email,
         isnull(dbo.funSYS_BuildName(mm.MemberID, 0, ''), '') AS FullName,
         vo.OfficeDescription,
         '/ContactUs?messageRecipient=Supreme%20Council: ' + vo.OfficeDescription AS ContactUs,
         isnull(mm.MemberID, 0) AS MemberID,
         'TblMasMembers/Details/' AS DirInfo,
         9 AS SortBy
FROM     tbl_ValOffices AS vo
         LEFT OUTER JOIN
         tbl_CorrMemberOffice AS cmo
         ON vo.OfficeID = cmo.OfficeID
            AND cmo.Year = dbo.funSYS_GetBegFratYearN(@NextYear)
         LEFT OUTER JOIN
         tbl_MasMembers AS mm
         ON cmo.MemberID = mm.MemberID
         LEFT OUTER JOIN AspNetUsers au on mm.KofCID=au.KofCMemberID
WHERE    vo.OfficeID = 41
ORDER BY SortBy;