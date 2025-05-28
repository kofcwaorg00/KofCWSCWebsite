alter PROCEDURE [dbo].[uspWEB_GetChairmen]
@NextYear BIT=0
AS
IF EXISTS (SELECT *
           FROM   tbl_MasMembers AS mm
                  INNER JOIN
                  tbl_CorrMemberOffice AS cmo
                  ON mm.MemberID = cmo.MemberID
                     AND cmo.Year = dbo.funSYS_GetBegFratYearN(@NextYear)
                  INNER JOIN
                  tbl_ValOffices AS vo
                  ON cmo.OfficeID = vo.OfficeID
           WHERE  vo.GroupID IN (5, 14))
    BEGIN
        SELECT   VO.AltDescription AS Chairmanship,
                 MM.FirstName + ' ' + MM.LastName AS FullName,
                 'mailto:' + isnull(VO.EmailAlias, 'Webmaster') + '@kofc-wa.org' AS Email,
                 '<a href=https://kofc-wa.org/ContactUs?messageRecipient=' + dbo.funSYS_URLEncode((SELECT ContactUSString
                                                                                                   FROM   tbl_valoffices
                                                                                                   WHERE  officeid = vo.OfficeID)) + '>' + (SELECT EmailAlias
                                                                                                                                            FROM   tbl_valoffices
                                                                                                                                            WHERE  officeid = vo.OfficeID) + '</a>' AS Email2,
                 MM.Council AS Council,
                 mm.MemberID,
                 'Washington State Directors/Chairmen ' + CAST (dbo.funSYS_GetBegFratYearN(@NextYear) AS VARCHAR) + ' - ' + CAST (dbo.funSYS_GetBegFratYearN(@NextYear) + 1 AS VARCHAR) AS Heading,
                 au.ProfilePictureUrl as Photo,
                 mm.KofCID
        FROM     tbl_MasMembers AS mm
                 INNER JOIN tbl_CorrMemberOffice AS cmo ON mm.MemberID = cmo.MemberID
                    AND cmo.Year = dbo.funSYS_GetBegFratYearN(@NextYear)
                 LEFT OUTER JOIN AspNetUsers au on au.KofCMemberID=mm.KofCID
                 INNER JOIN
                 tbl_ValOffices AS vo
                 ON cmo.OfficeID = vo.OfficeID
        WHERE    vo.GroupID IN (5, 14)
        ORDER BY VO.AltDescription;
    END
ELSE
    BEGIN
        SELECT '' AS Chairmanship,
               '' AS FullName,
               '' AS Email,
               '' AS Email2,
               0 AS Council,
               0 AS MemberID,
               'Washington State Directors/Chairmen ' + CAST (dbo.funSYS_GetBegFratYearN(@NextYear) AS VARCHAR) + ' - ' + CAST (dbo.funSYS_GetBegFratYearN(@NextYear) + 1 AS VARCHAR) AS Heading,
               NULL as Photo,
               NULL as KofCID
    END