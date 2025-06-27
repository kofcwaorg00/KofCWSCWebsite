alter PROCEDURE [dbo].[uspWEB_GetCouncils]
@Year INT=0
AS
SELECT   vc.CouncilNo AS CouncilNo,
         vc.District AS District,
         vc.City AS City,
         vc.CouncilName AS CouncilName,
         isnull(gk.GrandKnight, 'not submitted') AS GrandKnight,
         isnull(gk.GrandKnightID, 0) AS GKMemberID,
         isnull(fs.FinSec, 'not submitted') AS FinancialSecretary,
         isnull(fs.FinSecID, 0) AS FSMemberID,
         isnull(ch.Chaplain, 'not submitted') AS Chaplain,
         isnull(ch.ChaplainID, 0) AS ChapMemberID,
         CASE WHEN isnull(NULLIF (vc.MeetingInfo, ''), '') = '' THEN vc.Info ELSE vc.meetinginfo END AS MeetingInfo,
         CAST (vc.Chartered AS DATETIME) AS Chartered,
         vc.WebSiteURL,
         vc.BulletinURL,
         vc.DioceseID AS Diocese,
         'Washington State Councils ' + CAST (dbo.funSYS_GetBegFratYearN(@Year) AS VARCHAR) + ' - ' + CAST (dbo.funSYS_GetBegFratYearN(@Year) + 1 AS VARCHAR) AS Heading,
         vc.MeetAddress,
         vc.MailAddress,
         vc.PhyAddress
FROM     vewSYS_Councils AS vc
         LEFT OUTER JOIN
         vewSYS_GKs AS gk
         ON vc.CouncilNo = gk.councilno
            AND gk.year = dbo.funSYS_GetBegFratYearN(@Year)
         LEFT OUTER JOIN
         vewSYS_FSs AS fs
         ON vc.CouncilNo = fs.councilno
            AND fs.year = dbo.funSYS_GetBegFratYearN(@Year)
         LEFT OUTER JOIN
         vewSYS_Chaplains AS ch
         ON vc.councilno = ch.councilno
            AND ch.year = dbo.funSYS_GetBegFratYearN(@Year)
WHERE    vc.status = 'A'
ORDER BY vc.CouncilNo;