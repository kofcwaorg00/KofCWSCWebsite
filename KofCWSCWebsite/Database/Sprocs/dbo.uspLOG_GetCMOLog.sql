ALTER PROCEDURE [dbo].[uspLOG_GetCMOLog]
AS
BEGIN
-- select * from tbl_MasMembers where MemberID=18878
    SELECT   lcmo.ID,
             lcmo.ChangeType,
             lcmo.ChangeDate,
             lcmo.OfficeID,
             vo.OfficeDescription,
             lcmo.Year,
             lcmo.MemberID,
             mm.FirstName + ' ' + mm.LastName AS MemberName,
             mm.KofCID,
             lcmo.Processed,
             CASE WHEN mml.FirstName IS NULL then 'System' else mml.FirstName + ' ' + mml.LastName end AS UpdatedBy,
             lcmo.Updated
    FROM     tblLOG_CorrMemberOffice AS lcmo
             LEFT OUTER JOIN
             tbl_ValOffices AS vo
             ON lcmo.OfficeID = vo.OfficeID
             LEFT OUTER JOIN
             tbl_MasMembers AS mm
             ON lcmo.MemberID = mm.MemberID
             LEFT OUTER JOIN
             tbl_MasMembers AS mml
             ON lcmo.UpdatedBy = mml.KofCID
    WHERE    lcmo.Processed = 0
             AND vo.ExchangeMailType = 'DistributionList'
    ORDER BY ChangeDate;
END