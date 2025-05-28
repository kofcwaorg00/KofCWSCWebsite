-- DD = 13
--select * from tbl_ValOffices

alter VIEW vewSYS_DDs
AS
--select * from vewSYS_DDs where year = dbo.funSYS_GetBegFratYearN(0) order by District

SELECT [dbo].[funSYS_GetDDForDistYear](dy.District,dy.Year) AS MemberID,ISNULL(mm.FirstName+' '+mm.LastName,'Vacant') as FullName,dy.Year,dy.District,mm.Council,mm.KofCID
FROM dbo.funSYS_GetDDYears() dy
LEFT OUTER JOIN tbl_MasMembers mm on mm.MemberID=[dbo].[funSYS_GetDDForDistYear](dy.District,dy.Year)



