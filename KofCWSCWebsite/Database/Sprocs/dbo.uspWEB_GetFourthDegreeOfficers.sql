CREATE PROCEDURE [dbo].[uspWEB_GetFourthDegreeOfficers]
AS
BEGIN
    SELECT   mm.Phone,
             dbo.funSYS_BuildName(mm.MemberID, 0, 'P') AS FullName,
             vo.OfficeDescription AS Title,
             CASE mo.OfficeID WHEN 133 THEN '<a href=''mailto:' + vo.EmailAlias + '@kofc.org''>' + vo.EmailAlias + '@kofc.org</a>' ELSE '<a href=''mailto:' + vo.EmailAlias + '@kofc-wa.org''>' + vo.EmailAlias + '@kofc-wa.org</a>' END AS Email2,
             '<a href=https://kofc-wa.org/ContactUs?messageRecipient=' + dbo.funSYS_URLEncode((SELECT ContactUSString
                                                                                               FROM   tbl_valoffices
                                                                                               WHERE  officeid = vo.OfficeID)) + '>' + (SELECT EmailAlias
                                                                                                                                        FROM   tbl_valoffices
                                                                                                                                        WHERE  officeid = vo.OfficeID) + '</a>' AS Email,
             '/images/FourthDegOfficers/' + Replace(Replace(vo.OfficeDescription, ' ', ''), '/', '') + '.png' AS Photo,
             mo.OfficeID AS OfficeID,
             '/images/FourthDegOfficers/' + Replace(Replace(vo.OfficeDescription, ' ', ''), '/', '') + 'Graphic.png' AS ChairGraphic,
             sp.Data,
             vo.WebPageTagLine AS WebPageTagLine,
             vo.SupremeURL AS SupremeURL,
             mo.MemberID,
             'Washington State Fourth Degree Officers ' + CAST (dbo.funSYS_GetBegFratYear() AS VARCHAR) + ' - ' + CAST (dbo.funSYS_GetBegFratYear() + 1 AS VARCHAR) AS Heading
    FROM     tbl_CorrMemberOffice AS mo
             LEFT OUTER JOIN
             tbl_MasMembers AS mm
             ON mo.MemberID = mm.MemberID
             LEFT OUTER JOIN
             tbl_ValOffices AS vo
             ON mo.OfficeID = vo.OfficeID
             LEFT OUTER JOIN
             tblWEB_SelfPublish AS sp
             ON sp.OID = mo.OfficeID
    WHERE    mo.OfficeID IN (133, 54, 31, 184, 193, 248, 192, 246, 247, 191, 110, 156, 262, 134, 250, 289)
             AND mo.Year = dbo.funSYS_GetBegFratYear()
    ORDER BY vo.DirSortOrder;
END