alter PROCEDURE [dbo].[uspSYS_GetCouncilsByDistrict]
@District INT, @NextYear BIT=0
AS
BEGIN
-- exec [uspSYS_GetCouncilsByDistrict] 12,1
    DECLARE @Year AS INT;
    IF @NextYear = 0
        BEGIN
            SET @Year = dbo.funSYS_GetBegFratYear();
        END
    ELSE
        BEGIN
            SET @Year = dbo.funSYS_GetBegFratYear() + 1;
        END
    PRINT @Year;
    SELECT   vc.C_NUMBER AS Council,
             vc.C_LOCATION AS Location,
             ISNULL((SELECT mm.FirstName + ' ' + mm.LastName
              FROM   tbl_MasMembers AS MM
              WHERE  mm.MemberID = (SELECT MemberID
                                    FROM   vewSYS_DDs dd
                                    WHERE  dd.District = @District
                                           AND dd.Year = @Year)),'Vacant') AS FullName
    FROM     tbl_ValCouncils AS vc
    WHERE    vc.district = @District
    ORDER BY vc.C_NUMBER;
END