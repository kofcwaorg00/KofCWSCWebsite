ALTER PROCEDURE [dbo].[uspCVN_CreateCheckBatch]
@GroupID INT
AS
IF (@GroupID = 3)
    BEGIN
        PRINT 'DDs';
        DELETE tblCVN_TrxMPD
        WHERE  GroupID = @GroupID;
        INSERT tblCVN_TrxMPD
        SELECT   mm.MemberID,
                 mm.Council,
                 cmo.District,
                 vg.GroupName AS [Group],
                 vo.AltDescription AS Office,
                 mm.FirstName + ' ' + mm.LastName + ' ' + CAST (mm.KofCID AS VARCHAR) AS Payee,
                 0 AS CheckNumber,
                 GETDATE() AS CheckDate,
                 mm.SeatedDelegateDay1 AS Day1D1,
                 mm.SeatedDelegateDay2 AS Day2D1,
                 mm.SeatedDelegateDay3 AS Day3D1,
                 CASE WHEN mil.Mileage IS NULL THEN '<img src=/images/CheckboxDisabled.png width=25px >' ELSE CASE isnull(mm.SeatedDelegateDay1, 0) WHEN 0 THEN '<img src=/images/CheckboxUnChecked.png width=25px />' ELSE '<img src=/images/CheckboxChecked.png width=25px />' END END AS Day1GD1,
                 CASE WHEN mil.Mileage IS NULL THEN '<img src=/images/CheckboxDisabled.png width=25px >' ELSE CASE isnull(mm.SeatedDelegateDay2, 0) WHEN 0 THEN '<img src=/images/CheckboxUnChecked.png width=25px />' ELSE '<img src=/images/CheckboxChecked.png width=25px />' END END AS Day2GD1,
                 CASE WHEN mil.Mileage IS NULL THEN '<img src=/images/CheckboxDisabled.png width=25px >' ELSE CASE isnull(mm.SeatedDelegateDay3, 0) WHEN 0 THEN '<img src=/images/CheckboxUnChecked.png width=25px />' ELSE '<img src=/images/CheckboxChecked.png width=25px />' END END AS Day3GD1,
                 isnull(mil.Mileage, -1) AS Miles,
                 CASE WHEN (isnull(CAST (mm.SeatedDelegateDay1 AS INT), 0) + isnull(CAST (mm.SeatedDelegateDay2 AS INT), 0) + isnull(CAST (mm.SeatedDelegateDay3 AS INT), 0)) = 0 THEN 0 ELSE ((isnull(CAST (mm.SeatedDelegateDay1 AS INT), 0) + isnull(CAST (mm.SeatedDelegateDay2 AS INT), 0) + isnull(CAST (mm.SeatedDelegateDay3 AS INT), 0)) * cc.MPDDay) + (mil.Mileage * 2 * cc.MPDMile) END AS CheckTotal,
                 cc.Location AS Location,
                 CASE WHEN (CAST (isnull(mm.SeatedDelegateDay1, 0) AS INT)) + CAST (isnull(mm.SeatedDelegateDay2, 0) AS INT) + CAST (isnull(mm.SeatedDelegateDay3, 0) AS INT) > 0 THEN 1 ELSE 0 END AS PayMe,
                 '' AS CouncilStatus,
                 vg.GroupID,
                 0 AS Day1D2,
                 0 AS Day2D2,
                 0 AS Day3D2,
                 '' AS Day1GD2,
                 '' AS Day2GD2,
                 '' AS Day3GD2,
                 cc.CheckAccount,
                 cc.Category,
                 'Paid ' + CAST (CAST (isnull(mm.SeatedDelegateDay1, 0) AS INT) + CAST (isnull(mm.SeatedDelegateDay2, 0) AS INT) + CAST (isnull(mm.SeatedDelegateDay3, 0) AS INT) AS VARCHAR) + ' Delegate Days for ' + dbo.funSYS_GetMemberTitle(MM.MemberID) + ' @$' + CAST (cc.MPDDay AS VARCHAR) + '/day and ' + CAST (mil.Mileage * 2 AS VARCHAR) + ' Miles Round Trip @$' + CAST (cc.MPDMile AS VARCHAR) + '/mile' AS Memo,
                 0 AS HasDDDelegate
        FROM     tbl_CorrMemberOffice AS cmo
                 INNER JOIN
                 tbl_MasMembers AS mm
                 ON cmo.MemberID = mm.MemberID
                 INNER JOIN
                 tbl_ValCouncils AS vc
                 ON mm.Council = vc.C_NUMBER
                 INNER JOIN
                 tbl_ValOffices AS vo
                 ON cmo.OfficeID = vo.OfficeID
                 INNER JOIN
                 tbl_ValGroups AS vg
                 ON vo.GroupID = vg.GroupID
                 INNER JOIN
                 tblCVN_Control AS cc
                 ON cc.id = 1
                 LEFT OUTER JOIN
                 tblCVN_MasMileage AS mil
                 ON mil.Council = mm.Council
                    AND mil.Location = cc.Location
                 LEFT OUTER JOIN
                 tblCVN_TrxMPDArchive AS ta
                 ON ta.MemberID = mm.MemberID
                    AND ta.Year = dbo.funSYS_GetBegFratYearN(0)
        WHERE    vo.GroupID = 3
                 AND vo.OfficeID <> 129
                 AND cmo.Year = dbo.funSYS_GetBegFratYearN(0)
                 AND mm.FirstName + ' ' + mm.LastName <> 'To Be Announced'
                 AND ta.Year IS NULL
        ORDER BY cmo.District;
    END
ELSE
    IF (@GroupID = 25)
        BEGIN
            DELETE tblCVN_TrxMPD
            WHERE  GroupID = @GroupID;
            INSERT tblCVN_TrxMPD
            SELECT   0 AS MemberID,
                     vc.C_NUMBER,
                     vc.District,
                     'DELEGATES' AS [Group],
                     '' AS Office,
                     vc.C_Name + ' ' + CAST (vc.C_NUMBER AS VARCHAR) AS Payee,
                     0 AS CheckNumber,
                     GETDATE() AS CheckDate,
                     vc.SeatedDelegateDay1D1 AS Day1D1,
                     vc.SeatedDelegateDay2D1 AS Day2D1,
                     vc.SeatedDelegateDay3D1 AS Day3D1,
                     CASE WHEN mil.Mileage IS NULL THEN '<img src=/images/CheckboxDisabled.png width=25px >' ELSE CASE isnull(vc.SeatedDelegateDay1D1, 0) WHEN 0 THEN '<img src=/images/CheckboxUnChecked.png width=25px />' ELSE '<img src=/images/CheckboxChecked.png width=25px />' END END AS Day1GD1,
                     CASE WHEN mil.Mileage IS NULL THEN '<img src=/images/CheckboxDisabled.png width=25px >' ELSE CASE isnull(vc.SeatedDelegateDay2D1, 0) WHEN 0 THEN '<img src=/images/CheckboxUnChecked.png width=25px />' ELSE '<img src=/images/CheckboxChecked.png width=25px />' END END AS Day2GD1,
                     CASE WHEN mil.Mileage IS NULL THEN '<img src=/images/CheckboxDisabled.png width=25px >' ELSE CASE isnull(vc.SeatedDelegateDay3D1, 0) WHEN 0 THEN '<img src=/images/CheckboxUnChecked.png width=25px />' ELSE '<img src=/images/CheckboxChecked.png width=25px />' END END AS Day3GD1,
                     isnull(mil.Mileage, -1) AS Miles,
                     CASE WHEN (isnull(CAST (vc.SeatedDelegateDay1D1 AS INT), 0) + isnull(CAST (vc.SeatedDelegateDay2D1 AS INT), 0) + isnull(CAST (vc.SeatedDelegateDay3D1 AS INT), 0) + isnull(CAST (vc.SeatedDelegateDay1D2 AS INT), 0) + isnull(CAST (vc.SeatedDelegateDay2D2 AS INT), 0) + isnull(CAST (vc.SeatedDelegateDay3D2 AS INT), 0)) = 0 THEN 0 ELSE ((isnull(CAST (vc.SeatedDelegateDay1D1 AS INT), 0) + isnull(CAST (vc.SeatedDelegateDay2D1 AS INT), 0) + isnull(CAST (vc.SeatedDelegateDay3D1 AS INT), 0) + isnull(CAST (vc.SeatedDelegateDay1D2 AS INT), 0) + isnull(CAST (vc.SeatedDelegateDay2D2 AS INT), 0) + isnull(CAST (vc.SeatedDelegateDay3D2 AS INT), 0)) * cc.MPDDay) + (mil.Mileage * 2 * cc.MPDMile) END AS CheckTotal,
                     cc.Location AS Location,
                     CASE WHEN (CAST (isnull(vc.SeatedDelegateDay1D1, 0) AS INT)) + CAST (isnull(vc.SeatedDelegateDay2D1, 0) AS INT) + CAST (isnull(vc.SeatedDelegateDay3D1, 0) AS INT) > 0 THEN 1 ELSE 0 END AS PayMe,
                     vc.Status AS CouncilStatus,
                     25 AS GroupID,
                     vc.SeatedDelegateDay1D2 AS Day1D2,
                     vc.SeatedDelegateDay2D2 AS Day2D2,
                     vc.SeatedDelegateDay3D2 AS Day3D2,
                     CASE WHEN mil.Mileage IS NULL THEN '<img src=/images/CheckboxDisabled.png width=25px >' ELSE CASE isnull(vc.SeatedDelegateDay1D2, 0) WHEN 0 THEN '<img src=/images/CheckboxUnChecked.png width=25px />' ELSE '<img src=/images/CheckboxChecked.png width=25px />' END END AS Day1GD2,
                     CASE WHEN mil.Mileage IS NULL THEN '<img src=/images/CheckboxDisabled.png width=25px >' ELSE CASE isnull(vc.SeatedDelegateDay2D2, 0) WHEN 0 THEN '<img src=/images/CheckboxUnChecked.png width=25px />' ELSE '<img src=/images/CheckboxChecked.png width=25px />' END END AS Day2GD2,
                     CASE WHEN mil.Mileage IS NULL THEN '<img src=/images/CheckboxDisabled.png width=25px >' ELSE CASE isnull(vc.SeatedDelegateDay3D2, 0) WHEN 0 THEN '<img src=/images/CheckboxUnChecked.png width=25px />' ELSE '<img src=/images/CheckboxChecked.png width=25px />' END END AS Day3GD2,
                     cc.CheckAccount,
                     cc.Category,
                     'Paid ' + CAST (CAST (isnull(vc.SeatedDelegateDay1D1, 0) AS INT) + CAST (isnull(vc.SeatedDelegateDay1D2, 0) AS INT) + CAST (isnull(vc.SeatedDelegateDay2D1, 0) AS INT) + CAST (isnull(vc.SeatedDelegateDay2D2, 0) AS INT) + CAST (isnull(vc.SeatedDelegateDay3D1, 0) + CAST (isnull(vc.SeatedDelegateDay3D2, 0) AS INT) AS INT) AS VARCHAR) + ' Delegate Days for Council #' + CAST (vc.C_NUMBER AS VARCHAR) + ' - ' + vc.C_NAME + ' @$' + CAST (cc.MPDDay AS VARCHAR) + '/day and ' + CAST (mil.Mileage * 2 AS VARCHAR) + ' Miles Round Trip @$' + CAST (cc.MPDMile AS VARCHAR) + '/mile' AS Memo,
                     dbo.funCVN_HasDDDelegate(vc.c_number) AS HasDDDelegate
            FROM     tbl_ValCouncils AS vc
                     INNER JOIN
                     tblCVN_Control AS cc
                     ON cc.id = 1
                     LEFT OUTER JOIN
                     tblCVN_MasMileage AS mil
                     ON mil.Council = vc.C_NUMBER
                        AND mil.Location = cc.Location
                     LEFT OUTER JOIN
                     tblCVN_TrxMPDArchive AS ta
                     ON vc.C_NUMBER = ta.Council
                        AND ta.Year = dbo.funSYS_GetBegFratYearN(0)
                        AND ta.[Group] = 'DELEGATES'
            WHERE    vc.C_NUMBER > 0
                     AND ta.Council IS NULL
            ORDER BY vc.DISTRICT, vc.C_NUMBER;
        END