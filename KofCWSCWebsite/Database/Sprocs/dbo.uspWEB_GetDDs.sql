alter PROCEDURE [dbo].[uspWEB_GetDDs] 
@NextYear BIT=0
AS

--exec [uspWEB_GetDDs]
IF EXISTS (SELECT *
           --FROM   tbl_MasMembers AS MM
           --       INNER JOIN
           --       tbl_ValCouncils AS VC
           --       ON MM.Council = VC.C_NUMBER
           --       INNER JOIN
           --       tbl_ValDistricts AS VD
           --       ON VC.District = VD.District
           --       INNER JOIN
           --       tbl_CorrMemberOffice AS dd
           --       ON dd.memberid = mm.MemberID
           --          AND OfficeID = 13
           FROM vewSYS_DDs dd
           WHERE  dd.Year = dbo.funSYS_GetBegFratYearN(@NextYear))
    BEGIN
        SELECT   dd.District AS DistrictI,
                 CAST (dd.District AS VARCHAR) AS District,
                 dd.FullName + ISNULL('(' + CONVERT (VARCHAR, dd.Council) + ')','') AS FullName,
                 dbo.fun_GetDistrictCouncilList(dd.District) AS AssignedCouncils,
                 CASE dd.FullName WHEN 'Vacant' THEN '' ELSE '<a href=https://kofc-wa.org/ContactUs?messageRecipient=District%20Deputies:%20District%20' + CAST (dd.District AS VARCHAR) + ' target=_blank>Email</a>' END AS Email,
                 isnull(dd.MemberID,0) AS MemberID,
                 'Washington State District Deputies ' + CAST (dbo.funSYS_GetBegFratYearN(@NextYear) AS VARCHAR) + ' - ' + CAST (dbo.funSYS_GetBegFratYearN(@NextYear) + 1 AS VARCHAR) AS Heading,
				 dbo.[funSYS_GetPhotoURL](dd.KofCID) as Photo,
                 --au.ProfilePictureUrl as Photo,
				 ISNULL(dd.KofCID,0) as KofCID

        --FROM     tbl_MasMembers AS MM
        --         INNER JOIN
        --         tbl_ValCouncils AS VC
        --         ON MM.Council = VC.C_NUMBER
        --         INNER JOIN
        --         tbl_ValDistricts AS VD
        --         ON VC.District = VD.District
        --         INNER JOIN
        --         tbl_CorrMemberOffice AS dd
        --         ON dd.memberid = mm.MemberID
        --            AND OfficeID = 13
        FROM vewSYS_DDs dd
		LEFT OUTER JOIN AspNetUsers au on au.KofCMemberID=dd.kofcid
        WHERE    dd.Year = dbo.funSYS_GetBegFratYearN(@NextYear)
        ORDER BY DistrictI;
    END
ELSE
    BEGIN
        SELECT 0 AS DistrictI,
               '' AS District,
               '' AS FullName,
               '' AS AssignedCouncils,
               '' AS Email,
               0 AS MemberID,
               'Washington State District Deputies ' + CAST (dbo.funSYS_GetBegFratYearN(@NextYear) AS VARCHAR) + ' - ' + CAST (dbo.funSYS_GetBegFratYearN(@NextYear) + 1 AS VARCHAR) AS Heading,
			   NULL as Photo,
			   0 as KofCID
    END