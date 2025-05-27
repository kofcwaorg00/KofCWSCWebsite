alter PROCEDURE [dbo].[uspWEB_GetSOS]
@SOID INT = 0
AS
--select * from tbl_ValOffices where GroupID=2
--**************************************************************************************************
-- 7/21/2024 Tim Philomeno
-- added OID and Type to support AOI on front page
--**************************************************************************************************
BEGIN
    IF @SOID = 0
        BEGIN
            SELECT  -- '/images/SOs/' + Replace(vo.OfficeDescription, ' ', '') + '.png' AS Photo,
                    au.ProfilePictureUrl as Photo,
                     vo.EmailAlias + '@kofc-wa.org' AS Email,
                     dbo.funSYS_BuildName(mm.MemberID, 0, 'P') AS FullName,
                     vo.OfficeDescription,
                     '' AS TagLine,
                     p.Data,
                     CASE p.OID WHEN 47 THEN 'carousel-item active' ELSE 'carousel-item' END AS class,
                     CASE p.OID WHEN 47 THEN 1 WHEN 46 THEN 2 WHEN 49 THEN 3 WHEN 50 THEN 4 WHEN 45 THEN 5 WHEN 51 THEN 6 WHEN 30 THEN 7 ELSE 0 END AS SortOrder,
                     p.URL,p.OID,'SO' as Type,
					 '' as Title,'' as GraphicURL,'' as LinkURL,'' as PostedDate,0 as Expired
            FROM     tblWEB_SelfPublish AS p 
                    LEFT OUTER JOIN tbl_CorrMemberOffice AS mo ON mo.OfficeID = p.OID
                    LEFT OUTER JOIN tbl_ValOffices AS vo ON vo.OfficeID = p.OID
                    LEFT OUTER JOIN tbl_MasMembers AS mm ON mo.MemberID = mm.MemberID
                    LEFT OUTER JOIN AspNetUsers au on mm.KofCID=au.KofCMemberID
            WHERE    p.OID IN (47, 49, 50, 45, 51, 30, 46)
                     AND mo.Year = dbo.funSYS_GetBegFratYear()
            ORDER BY CASE p.OID WHEN 47 THEN 1 WHEN 46 THEN 2 WHEN 49 THEN 3 WHEN 50 THEN 4 WHEN 45 THEN 5 WHEN 51 THEN 6 WHEN 30 THEN 7 ELSE 0 END;
        END
    ELSE
        BEGIN
            SELECT --'/images/SOs/' + Replace(vo.OfficeDescription, ' ', '') + '.png' AS Photo,
                    au.ProfilePictureUrl as Photo,
                   vo.EmailAlias + '@kofc-wa.org' AS Email,
                   dbo.funSYS_BuildName(mm.MemberID, 0, 'P') AS FullName,
                   vo.OfficeDescription,
                   '' AS TagLine,
                   p.Data,
                   '' AS class,
                   p.URL,p.OID,'SO' as Type,
				   '' as Title,'' as GraphicURL,'' as LinkURL,'' as PostedDate,0 as Expired
            FROM   tblWEB_SelfPublish AS p
                    LEFT OUTER JOIN tbl_CorrMemberOffice AS mo ON mo.OfficeID = p.OID
                    LEFT OUTER JOIN tbl_ValOffices AS vo ON vo.OfficeID = p.OID
                    LEFT OUTER JOIN tbl_MasMembers AS mm ON mo.MemberID = mm.MemberID
                    LEFT OUTER JOIN AspNetUsers au on mm.KofCID=au.KofCMemberID
            WHERE  p.OID = @SOID
                   AND mo.Year = dbo.funSYS_GetBegFratYear();
        END
END