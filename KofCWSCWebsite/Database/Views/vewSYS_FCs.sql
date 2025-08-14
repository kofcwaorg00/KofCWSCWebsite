CREATE VIEW [dbo].[vewSYS_FCs]
AS
-- select * from [vewSYS_FCs]
SELECT va.A_NUMBER AS AssemblyNo,
       va.A_LOCATION AS Location,
       va.A_NAME AS Name,
       MM.FirstName + ' ' + MM.LastName AS FC,
       '#' AS FCURL,
       email AS FCEmail,
       mm.MemberID AS FCID,
       CASE WHEN DATEPART(mm, GETDATE()) >= 6 THEN CASE WHEN LastUpdated > CAST ('06/01/' + CAST (datepart(yy, getdate()) AS VARCHAR) AS DATETIME) THEN 1 ELSE 0 END ELSE 1 END AS FCUpdatedForNewFratYear,
       co.Year
FROM   tbl_MasMembers AS mm
       INNER JOIN
       tbl_CorrMemberOffice AS co
       ON co.MemberID = mm.MemberID
          AND co.OfficeID = 17
       LEFT OUTER JOIN
       tbl_ValAssys AS va
       ON mm.Assembly = va.a_number
WHERE  isnull(mm.Deceased, 0) <> 1;