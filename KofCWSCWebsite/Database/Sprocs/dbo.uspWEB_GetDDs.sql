ALTER PROCEDURE [dbo].[uspWEB_GetDDs] 
@NextYear BIT=0
AS

--exec [uspWEB_GetDDs] 0
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
                 CASE dd.district WHEN 0 then '' else CAST (dd.District AS VARCHAR) END AS District,
                 dd.FullName + ISNULL('(' + CONVERT (VARCHAR, dd.Council) + ')','') AS FullName,
                 CASE dd.district WHEN 0 THEN 'District Deputy Director all Districts' ELSE dbo.fun_GetDistrictCouncilList(dd.District) END AS AssignedCouncils,
                 CASE dd.FullName WHEN 'Vacant' THEN '' ELSE '<a href=https://kofc-wa.org/ContactUs?messageRecipient=District%20Deputies:%20District%20' + CAST (dd.District AS VARCHAR) + ' target=_blank>Email</a>' END AS Email,
                 isnull(dd.MemberID,0) AS MemberID,
                 'Washington State District Deputies ' + CAST (dbo.funSYS_GetBegFratYearN(@NextYear) AS VARCHAR) + ' - ' + CAST (dbo.funSYS_GetBegFratYearN(@NextYear) + 1 AS VARCHAR) AS Heading
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
               'Washington State District Deputies ' + CAST (dbo.funSYS_GetBegFratYearN(@NextYear) AS VARCHAR) + ' - ' + CAST (dbo.funSYS_GetBegFratYearN(@NextYear) + 1 AS VARCHAR) AS Heading;
    END