alter PROCEDURE [dbo].[uspWEB_GetChairmanInfoBlock]
@OfficeID INT, @NextYear INT=0
AS
BEGIN
-- exec [uspWEB_GetChairmanInfoBlock] 8,0
    IF EXISTS (SELECT *
               FROM   tbl_MasMembers AS mm
               WHERE  memberid = (SELECT MemberID
                                  FROM   tbl_CorrMemberOffice
                                  WHERE  OfficeID = @OfficeID
                                         AND year = dbo.funSYS_GetBegFratYearN(@NextYear)))
        BEGIN
            SELECT mm.Phone,
                   dbo.funSYS_BuildName(mm.MemberID, 0, 'P') AS FullName,
                   (SELECT OfficeDescription
                    FROM   tbl_ValOffices
                    WHERE  officeid = @OfficeID) AS Title,
                   CASE @OfficeID WHEN 133 THEN '<a href=''mailto:' + (SELECT EmailAlias
                                                                       FROM   tbl_valoffices
                                                                       WHERE  officeid = @OfficeID) + '@kofc.org''>' + (SELECT EmailAlias
                                                                                                                        FROM   tbl_valoffices
                                                                                                                        WHERE  officeid = @OfficeID) + '@kofc.org</a>' ELSE '<a href=''mailto:' + (SELECT EmailAlias
                                                                                                                                                                                                   FROM   tbl_valoffices
                                                                                                                                                                                                   WHERE  officeid = @OfficeID) + '@kofc-wa.org''>' + (SELECT EmailAlias
                                                                                                                                                                                                                                                       FROM   tbl_valoffices
                                                                                                                                                                                                                                                       WHERE  officeid = @OfficeID) + '@kofc-wa.org</a>' END AS Email,
                    au.ProfilePictureURL as Photo,
                   --'/images/SCs/' + Replace(Replace((SELECT OfficeDescription
                   --                                  FROM   tbl_ValOffices
                   --                                  WHERE  officeid = @OfficeID), ' ', ''), '/', '') + '.png' AS Photo,
                   @OfficeID AS OfficeID,
                   '/images/SCs/' + Replace(Replace((SELECT OfficeDescription
                                                     FROM   tbl_ValOffices
                                                     WHERE  officeid = @OfficeID), ' ', ''), '/', '') + 'Graphic.png' AS ChairGraphic,
                   sp.Data,
                   (SELECT WebPageTagLine
                    FROM   tbl_ValOffices
                    WHERE  OfficeID = @OfficeID) AS WebPageTagLine,
                   (SELECT SupremeURL
                    FROM   tbl_ValOffices
                    WHERE  OfficeID = @OfficeID) AS SupremeURL,
                   mm.MemberID,
                   (SELECT OfficeDescription
                    FROM   tbl_ValOffices
                    WHERE  OfficeID = @OfficeID) + ' ' + CAST (dbo.funSYS_GetBegFratYearN(@NextYear) AS VARCHAR) + '-' + CAST ((dbo.funSYS_GetBegFratYearN(@NextYear) + 1) AS VARCHAR) AS Heading,
                    mm.KofCID
            FROM   tbl_MasMembers AS mm
                   LEFT OUTER JOIN tblWEB_SelfPublish AS sp ON sp.OID = @OfficeID
                   LEFT OUTER JOIN AspNetUsers au on au.KofCMemberID=mm.KofCID
            WHERE  memberid = (SELECT MemberID
                               FROM   tbl_CorrMemberOffice
                               WHERE  OfficeID = @OfficeID
                                      AND Year = dbo.funSYS_GetBegFratYearN(@NextYear));
        END
    ELSE
        BEGIN
            SELECT '' AS Phone,
                   'no chairman assigned' AS FullName,
                   (SELECT OfficeDescription
                    FROM   tbl_ValOffices
                    WHERE  OfficeID = @OfficeID) AS Title,
                   (SELECT EmailAlias
                    FROM   tbl_ValOffices
                    WHERE  OfficeID = @OfficeID) AS Email,
                    null as Photo,
                   --'/images/SCs/' + Replace(Replace((SELECT OfficeDescription
                   --                                  FROM   tbl_ValOffices
                   --                                  WHERE  officeid = @OfficeID), ' ', ''), '/', '') + '.png' AS Photo,
                   @OfficeID AS OfficeID,
                   '/images/SCs/' + Replace(Replace((SELECT OfficeDescription
                                                     FROM   tbl_ValOffices
                                                     WHERE  officeid = @OfficeID), ' ', ''), '/', '') + 'Graphic.png' AS ChairGraphic,
                   (SELECT data
                    FROM   tblWEB_SelfPublish
                    WHERE  oid = @OfficeID) AS Data,
                   (SELECT WebPageTagLine
                    FROM   tbl_ValOffices
                    WHERE  OfficeID = @OfficeID) AS WebPageTagLine,
                   (SELECT SupremeURL
                    FROM   tbl_ValOffices
                    WHERE  OfficeID = @OfficeID) AS SupremeURL,
                   0 AS MemberId,
                   'No Chairman Assigned' + ' ' + CAST (dbo.funSYS_GetBegFratYearN(@NextYear) AS VARCHAR) + '-' + CAST ((dbo.funSYS_GetBegFratYearN(@NextYear) + 1) AS VARCHAR) AS Heading,
                   0 as KofCID
        END
END