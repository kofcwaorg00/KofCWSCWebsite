ALTER PROCEDURE [dbo].[uspCVN_PrintMPDChecks]
@Group INT, @StartCheckNo INT, @PrintCheckNumber BIT
AS
WITH CTE
AS   (SELECT ID,
             CheckNumber,
             @StartCheckNo + ROW_NUMBER() OVER (ORDER BY District, Council) - 1 AS NewCheckNumber
      FROM   tblCVN_TrxMPD
      WHERE  GroupID = @Group
             AND CheckTotal > 0)
UPDATE CTE
SET    CheckNumber = NewCheckNumber;
IF (@Group = 3)
    BEGIN
        SELECT   TX.Payee,
                 tx.CheckNumber,
                 TX.CheckTotal,
                 MM.Address,
                 MM.City,
                 MM.State,
                 MM.PostalCode,
                 TX.Council,
                 TX.District,
                 dbo.funSYS_GetMemberTitle(MM.MemberID) AS Title,
                 TX.CheckDate,
                 TX.Miles,
                 CAST (isnull(tx.Day1D1, 0) AS INT) + CAST (isnull(tx.Day2D1, 0) AS INT) + CAST (isnull(tx.Day3D1, 0) AS INT) AS SeatedDays,
                 'https://kofcwa.sharepoint.com/:i:/s/WebsiteStorage/EV4gWcXWUVlAgW3_FuAjq2oBKXKMM94yToiU6DIiLBfrpA?e=PSVGBn' AS SigImageID,
                 @PrintCheckNumber AS PrintCheckNumber,
                 tx.Memo
        FROM     tblCVN_TrxMPD AS TX
                 INNER JOIN
                 tbl_MasMembers AS MM
                 ON TX.MemberID = MM.MemberID
        WHERE    TX.GroupID = @Group
                 AND TX.CheckTotal > 0
        ORDER BY TX.District, TX.Council;
    END
ELSE
    IF (@Group = 25)
        BEGIN
            SELECT   TX.Payee,
                     tx.CheckNumber,
                     TX.CheckTotal,
                     vc.MailAddress AS Address,
                     vc.MailCity AS City,
                     vc.MailState AS State,
                     vc.MailPostalCode AS PostalCode,
                     TX.Council,
                     TX.DISTRICT,
                     '' AS Title,
                     TX.CheckDate,
                     TX.Miles,
                     CAST (isnull(tx.Day1D1, 0) AS INT) + CAST (isnull(tx.Day2D1, 0) AS INT) + CAST (isnull(tx.Day3D1, 0) AS INT) + CAST (isnull(tx.Day1D2, 0) AS INT) + CAST (isnull(tx.Day2D2, 0) AS INT) + CAST (isnull(tx.Day3D2, 0) AS INT) AS SeatedDays,
                     'https://kofcwa.sharepoint.com/:i:/s/WebsiteStorage/EV4gWcXWUVlAgW3_FuAjq2oBKXKMM94yToiU6DIiLBfrpA?e=PSVGBn' AS SigImageID,
                     @PrintCheckNumber AS PrintCheckNumber,
                     tx.Memo
            FROM     tblCVN_TrxMPD AS TX
                     INNER JOIN
                     tbl_ValCouncils AS VC
                     ON TX.Council = VC.C_NUMBER
            WHERE    TX.GroupID = @Group
                     AND TX.CheckTotal > 0
            ORDER BY TX.District, tX.Council;
        END