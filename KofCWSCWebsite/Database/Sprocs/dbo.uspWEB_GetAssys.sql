alter PROCEDURE [dbo].[uspWEB_GetAssys]
@Year INT=0
AS
SELECT   va.A_NUMBER AS AssyNo,
         va.A_LOCATION AS City,
         va.A_NAME AS AssyName,
         isnull(fn.FN, 'not submitted') AS FN,
         isnull(fn.FNID, 0) AS FNID,
         isnull(fc.FC, 'not submitted') AS FC,
         isnull(fc.FCID, 0) AS FCID,
         isnull(ff.FF, 'not submitted') AS FF,
         isnull(ff.FFID, 0) AS FFID,
         va.WebSiteURL,
         va.MasterLoc AS ML,
         dbo.funsys_ConcatMeeting(va.[ADD INFO 1], va.[ADD INFO 2], va.[ADD INFO 3]) AS MeetingInfo,
         'Washington State Fourth Degree Assemblies ' + CAST (dbo.funSYS_GetBegFratYearN(@Year) AS VARCHAR) + ' - ' + CAST (dbo.funSYS_GetBegFratYearN(@Year) + 1 AS VARCHAR) AS Heading
FROM     tbl_ValAssys AS va
         LEFT OUTER JOIN
         vewSYS_fNs AS fn
         ON FN.AssemblyNo = va.A_NUMBER
            AND fn.year = dbo.funSYS_GetBegFratYearN(@Year)
         LEFT OUTER JOIN
         vewSYS_FCs AS fc
         ON fc.AssemblyNo = va.A_NUMBER
            AND fc.year = dbo.funSYS_GetBegFratYearN(@Year)
         LEFT OUTER JOIN
         vewSYS_FFs AS ff
         ON ff.AssemblyNo = va.A_NUMBER
            AND ff.year = dbo.funSYS_GetBegFratYearN(@Year)
WHERE va.Status = 'A'
ORDER BY va.A_NUMBER;