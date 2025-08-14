create PROCEDURE uspSYS_GetEmailAddrForBulkSendTo
@KofCID INT,@NextYear int = 0

AS

-- exec uspSYS_GetEmailAddrForBulkSendTo 5055438,0-- 2332439-- 5055438 
--select * from tbl_MasMembers where lastname like '%dunn%'
--select * from tbl_ValOffices where OfficeDescription like '%communic%'


SELECT CASE vo.EmailAlias WHEN 'DD' THEN vo.EmailAlias + CAST (cmo.District AS NVARCHAR) + '@kofc-wa.org' ELSE vo.EmailAlias + '@kofc-wa.org' END AS Email
FROM   tbl_CorrMemberOffice AS cmo
       LEFT OUTER JOIN
       tbl_ValOffices AS vo
       ON cmo.OfficeID = vo.OfficeID
       LEFT OUTER JOIN
       tbl_MasMembers AS mm
       ON cmo.MemberID = mm.MemberID
WHERE  cmo.Year = dbo.funSYS_GetBegFratYearN(@NextYear)
       AND vo.GroupID IN (2, 3, 5, 8, 14)
       AND mm.KofCID = @KofCID
ORDER BY vo.EmailAlias