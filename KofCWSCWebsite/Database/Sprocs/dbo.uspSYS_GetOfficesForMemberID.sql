CREATE PROCEDURE [dbo].[uspSYS_GetOfficesForMemberID]
@MemberID INT
AS
BEGIN
    SELECT cmo.ID,
           cmo.MemberID,
           cmo.OfficeID,
           vo.OfficeDescription,
           cmo.PrimaryOffice,
           cmo.Year,
           cmo.Assembly,
           cmo.Council,
           cmo.District
    FROM   tbl_CorrMemberOffice AS cmo
           INNER JOIN
           tbl_MasMembers AS mm
           ON cmo.MemberID = mm.MemberID
           INNER JOIN
           tbl_ValOffices AS vo
           ON cmo.OfficeID = vo.OfficeID
    WHERE  cmo.MemberID = @MemberID;
END