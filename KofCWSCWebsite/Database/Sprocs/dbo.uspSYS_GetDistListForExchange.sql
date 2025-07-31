alter PROCEDURE [dbo].[uspSYS_GetDistListForExchange]
@Type NVARCHAR (10)='AGWM', @GroupID INT, @OfficeID INT, @NextYear INT=0
AS
BEGIN

-- for a single sc, run AGWM,14,<OID>,0 then AGWG,14,0,0
-- exec [uspSYS_GetDistListForExchange] 'AGWM',3,0,0 -- gets all dds and adds them
-- exec [uspSYS_GetDistListForExchange] 'AGWG',3,0,0 -- adds the dd groups to all dds
-- exec [uspSYS_GetDistListForExchange] 'AGWM',14,204,0 -- gets all scs
-- exec [uspSYS_GetDistListForExchange] 'AGWG',14,204,0 -- gets all scs
-- exec [uspSYS_GetDistListForExchange] 'AGWM',12,0,0 -- gets all council officers
-- exec [uspSYS_GetDistListForExchange] 'AGWG',12,0,0 -- gets all council officers
-- exec [uspSYS_GetDistListForExchange] 'AGWM',16,0,0 -- gets all assembly officers
-- exec [uspSYS_GetDistListForExchange] 'AGWG',16,0,0 -- gets all assembly officers
-- select * from tbl_ValOffices where officedescription like '%shining%'
-- select * from tbl_ValGroups
----------------------------------------------------------------------------------
-- 5/20/2025 Tim Philomeno
-- I am looking at this to switch it over to using vewSYS_DDs but I will not do
-- it at this time.  This is so complicated I would need to start from scratch
----------------------------------------------------------------------------------
    DECLARE @ManagedBy AS VARCHAR (50), @PreFix AS NVARCHAR (10);
    SET @ManagedBy = 'dnguyen@kofc-wa.org';
    SET @PreFix = '';
    IF @Type = 'AGWM'
        BEGIN
            PRINT 'ALL';
            SELECT   LEFT(@Prefix + 
                        CASE
                            WHEN vo.OfficeID = 129 THEN 'DDD'
                            WHEN vo.groupid = 3 THEN 'DD' + dbo.funSYS_PaddWZeros(CAST (cmo.DISTRICT AS VARCHAR), 2) 
                            WHEN vo.OfficeID = 22 THEN 'FS' + CAST (mm.council AS VARCHAR) 
                            WHEN vo.OfficeID = 27 THEN 'GK' + CAST (mm.council AS VARCHAR) 
                            WHEN vo.OfficeID = 20 THEN 'FN' + CAST (mm.Assembly AS VARCHAR) 
                            WHEN vo.OfficeID = 16 THEN 'All Field Agents' 
                            WHEN vo.GroupID = 8 THEN 'All Past State Deputies' 
                            WHEN vo.OfficeID = 17 THEN 'FC' + CAST (mm.Assembly AS VARCHAR) 
                            ELSE vo.OfficeDescription END, 64) AS GroupName,
                     @Prefix + 
                        CASE 
                            WHEN vo.OfficeID = 129 THEN 'DDD@kofc-wa.org'
                            WHEN vo.groupid = 3 THEN 'DD' + dbo.funSYS_PaddWZeros(CAST (cmo.DISTRICT AS VARCHAR), 2) + '@kofc-wa.org' 
                            WHEN vo.OfficeID = 16 THEN 'AllFAs@kofc-wa.org' 
                            WHEN vo.groupid = 8 THEN 'AllPSDs@kofc-wa.org' 
                            WHEN vo.OfficeID = 22 THEN 'FS' + CAST (mm.council AS VARCHAR) + '@kofc-wa.org' 
                            WHEN vo.OfficeID = 27 THEN 'GK' + CAST (mm.council AS VARCHAR) + '@kofc-wa.org' 
                            WHEN vo.OfficeID = 20 THEN 'FN' + CAST (mm.Assembly AS VARCHAR) + '@kofc-wa.org' 
                            WHEN vo.OfficeID = 17 THEN 'FC' + CAST (mm.Assembly AS VARCHAR) + '@kofc-wa.org' 
                            ELSE vo.EmailAlias + '@kofc-wa.org' END AS GroupEmail,
                     mm.FirstName + ' ' + mm.LastName AS RecipientName,
                     mm.Email AS RecipientEmail,
                     @ManagedBy AS ManagedBy,
                     CASE 
                        WHEN vo.OfficeID = 129 THEN 'DDD@kofc-wa.org'
                        WHEN vo.groupid = 3 THEN 'DD' + CAST (cmo.DISTRICT AS VARCHAR)  + '@kofc-wa.org' 
                        WHEN vo.OfficeID = 22 THEN 'FS' + CAST (mm.council AS VARCHAR) 
                        WHEN vo.OfficeID = 27 THEN 'GK' + CAST (mm.council AS VARCHAR) 
                        WHEN vo.OfficeID = 20 THEN 'FN' + CAST (mm.Assembly AS VARCHAR) 
                        WHEN vo.OfficeID = 17 THEN 'FC' + CAST (mm.Assembly AS VARCHAR) 
                        WHEN vo.OfficeID = 16 THEN 'FA' + mm.FirstName + mm.LastName 
                        ELSE vo.EmailAlias END AS Alias
            FROM     tbl_CorrMemberOffice AS cmo
                     INNER JOIN
                     tbl_MasMembers AS mm
                     ON cmo.MemberID = mm.MemberID
                     INNER JOIN
                     tbl_ValOffices AS vo
                     ON cmo.OfficeID = vo.OfficeID
                     INNER JOIN
                     tbl_ValCouncils AS vc
                     ON mm.Council = vc.C_NUMBER
            WHERE    cmo.Year = dbo.funSYS_GetBegFratYearN(@NextYear)
                     AND mm.KofCID NOT IN (9999990, 9999993, 9999987, 9999903)
                     AND isnull(mm.Deceased, 0) <> 1
                     AND vo.EmailAlias IS NOT NULL
                     AND vo.ExchangeMailType = 'DistributionList'
                     AND (@GroupID = 0
                          OR vo.GroupID = @GroupID)
                     AND (@OfficeID = 0
                          OR vo.OfficeID = @OfficeID)
            ORDER BY CASE WHEN vo.groupid = 3 THEN 'DD' + dbo.funSYS_PaddWZeros(CAST (cmo.DISTRICT AS VARCHAR), 2) WHEN vo.OfficeID = 22 THEN 'FS' + CAST (mm.council AS VARCHAR) WHEN vo.OfficeID = 27 THEN 'GK' + CAST (mm.council AS VARCHAR) WHEN vo.OfficeID = 20 THEN 'FN' + CAST (mm.Assembly AS VARCHAR) WHEN vo.OfficeID = 16 THEN 'All Field Agents' WHEN vo.GroupID = 8 THEN 'All Past State Deputies' WHEN vo.OfficeID = 17 THEN 'FC' + CAST (mm.Assembly AS VARCHAR) ELSE vo.OfficeDescription END;
        END
    IF @Type = 'AGWG'
        BEGIN
            PRINT 'AGWG';
            SELECT Results.GroupName,
                   Results.GroupEmail,
                   Results.RecipientName,
                   Results.RecipientEmail,
                   Results.ManagedBy,
                   Results.Alias
            FROM   (SELECT @Prefix + 'All District Deputies' AS GroupName,
                           @Prefix + 'AllDDs@KOFC-WA.ORG' AS GroupEmail,
                           @Prefix + 'DD' + dbo.funSYS_PaddWZeros(CAST (cmo.District AS NVARCHAR),2) AS RecipientName,
                           @Prefix + 'DD' + dbo.funSYS_PaddWZeros(CAST (cmo.District AS NVARCHAR),2) + '@KOFC-WA.ORG' AS RecipientEmail,
                           @ManagedBy AS ManagedBy,
                           'DD' + CAST (cmo.District AS NVARCHAR) AS Alias,
                           vo.GroupID,
                           vo.OfficeID
                    FROM   tbl_CorrMemberOffice AS cmo
                           INNER JOIN
                           tbl_ValOffices AS vo
                           ON cmo.OfficeID = vo.OfficeID
                           INNER JOIN
                           tbl_MasMembers AS mm
                           ON cmo.MemberID = mm.MemberID
                    WHERE  cmo.OfficeID = 13
                           AND cmo.Year = dbo.funSYS_GetBegFratYearN(0)
                    UNION ALL
                    SELECT @Prefix + 'All FIA Directors' AS GroupName,
                           @Prefix + 'ALLFIAS@KOFC-WA.ORG' AS GroupEmail,
                           @Prefix + vo.OfficeDescription AS RecipientName,
                           @Prefix + vo.EmailAlias + '@KOFC-WA.ORG' AS RecipientEmail,
                           @ManagedBy AS ManagedBy,
                           vo.EmailAlias,
                           vo.GroupID,
                           vo.OfficeID
                    FROM   tbl_CorrMemberOffice AS cmo
                           INNER JOIN
                           tbl_ValOffices AS vo
                           ON cmo.OfficeID = vo.OfficeID
                           INNER JOIN
                           tbl_MasMembers AS mm
                           ON cmo.MemberID = mm.MemberID
                    WHERE  vo.GroupID = 5
                           AND cmo.Year = dbo.funSYS_GetBegFratYearN(0)
                    UNION ALL
                    SELECT @Prefix + 'All State Directors & Chairmen' AS GroupName,
                           @Prefix + 'ALLSCS@KOFC-WA.ORG' AS GroupEmail,
                           @Prefix + vo.OfficeDescription AS RecipientName,
                           @Prefix + vo.EmailAlias + '@KOFC-WA.ORG' AS RecipientEmail,
                           @ManagedBy AS ManagedBy,
                           vo.EmailAlias,
                           vo.GroupID,
                           vo.OfficeID
                    FROM   tbl_CorrMemberOffice AS cmo
                           INNER JOIN
                           tbl_ValOffices AS vo
                           ON cmo.OfficeID = vo.OfficeID
                           INNER JOIN
                           tbl_MasMembers AS mm
                           ON cmo.MemberID = mm.MemberID
                    WHERE  vo.GroupID IN (5, 14)
                           AND cmo.Year = dbo.funSYS_GetBegFratYearN(0)
                    UNION ALL
                    SELECT @Prefix + 'All State Officers' AS GroupName,
                           @Prefix + 'ALLSOS@KOFC-WA.ORG' AS GroupEmail,
                           @Prefix + vo.OfficeDescription AS RecipientName,
                           @Prefix + vo.EmailAlias + '@KOFC-WA.ORG' AS RecipientEmail,
                           @ManagedBy AS ManagedBy,
                           vo.EmailAlias,
                           vo.GroupID,
                           vo.OfficeID
                    FROM   tbl_CorrMemberOffice AS cmo
                           INNER JOIN
                           tbl_ValOffices AS vo
                           ON cmo.OfficeID = vo.OfficeID
                           INNER JOIN
                           tbl_MasMembers AS mm
                           ON cmo.MemberID = mm.MemberID
                    WHERE  vo.GroupID IN (2)
                           AND cmo.Year = dbo.funSYS_GetBegFratYearN(0)
                    UNION ALL
                    SELECT @Prefix + 'All State Admin & Finance Trainers' AS GroupName,
                           @Prefix + 'ALLSTATETRAINERS@KOFC-WA.ORG' AS GroupEmail,
                           @Prefix + vo.OfficeDescription AS RecipientName,
                           @Prefix + vo.EmailAlias + '@KOFC-WA.ORG' AS RecipientEmail,
                           @ManagedBy AS ManagedBy,
                           vo.EmailAlias,
                           vo.GroupID,
                           vo.OfficeID
                    FROM   tbl_CorrMemberOffice AS cmo
                           INNER JOIN
                           tbl_ValOffices AS vo
                           ON cmo.OfficeID = vo.OfficeID
                           INNER JOIN
                           tbl_MasMembers AS mm
                           ON cmo.MemberID = mm.MemberID
                    WHERE  vo.Officeid IN (225, 226, 227, 228, 229, 230, 223, 224, 286)
                           AND cmo.Year = dbo.funSYS_GetBegFratYearN(0)
                    UNION ALL
                    SELECT @Prefix + 'All FSs' AS GroupName,
                           @Prefix + 'ALLFSS@KOFC-WA.ORG' AS GroupEmail,
                           @Prefix + vo.OfficeDescription + CAST (mm.Council AS NVARCHAR) AS RecipientName,
                           @Prefix + vo.EmailAlias + CAST (mm.Council AS NVARCHAR) + '@KOFC-WA.ORG' AS RecipientEmail,
                           @ManagedBy AS ManagedBy,
                           vo.EmailAlias,
                           vo.GroupID,
                           vo.OfficeID
                    FROM   tbl_CorrMemberOffice AS cmo
                           INNER JOIN
                           tbl_ValOffices AS vo
                           ON cmo.OfficeID = vo.OfficeID
                           INNER JOIN
                           tbl_MasMembers AS mm
                           ON cmo.MemberID = mm.MemberID
                    WHERE  vo.Officeid IN (22)
                           AND cmo.Year = dbo.funSYS_GetBegFratYearN(0)
                    UNION ALL
                    SELECT @Prefix + 'All GKs' AS GroupName,
                           @Prefix + 'ALLGKS@KOFC-WA.ORG' AS GroupEmail,
                           @Prefix + vo.OfficeDescription + CAST (mm.Council AS NVARCHAR) AS RecipientName,
                           @Prefix + vo.EmailAlias + CAST (mm.Council AS NVARCHAR) + '@KOFC-WA.ORG' AS RecipientEmail,
                           @ManagedBy AS ManagedBy,
                           vo.EmailAlias,
                           vo.GroupID,
                           vo.OfficeID
                    FROM   tbl_CorrMemberOffice AS cmo
                           INNER JOIN
                           tbl_ValOffices AS vo
                           ON cmo.OfficeID = vo.OfficeID
                           INNER JOIN
                           tbl_MasMembers AS mm
                           ON cmo.MemberID = mm.MemberID
                    WHERE  vo.Officeid IN (27)
                           AND cmo.Year = dbo.funSYS_GetBegFratYearN(0)
                    UNION ALL
                    SELECT @Prefix + 'All FNs' AS GroupName,
                           @Prefix + 'ALLFNS@KOFC-WA.ORG' AS GroupEmail,
                           @Prefix + vo.OfficeDescription + CAST (mm.Assembly AS NVARCHAR) AS RecipientName,
                           @Prefix + vo.EmailAlias + CAST (mm.Assembly AS NVARCHAR) + '@KOFC-WA.ORG' AS RecipientEmail,
                           @ManagedBy AS ManagedBy,
                           vo.EmailAlias,
                           vo.GroupID,
                           vo.OfficeID
                    FROM   tbl_CorrMemberOffice AS cmo
                           INNER JOIN
                           tbl_ValOffices AS vo
                           ON cmo.OfficeID = vo.OfficeID
                           INNER JOIN
                           tbl_MasMembers AS mm
                           ON cmo.MemberID = mm.MemberID
                    WHERE  vo.Officeid IN (20)
                           AND cmo.Year = dbo.funSYS_GetBegFratYearN(0)
                    UNION ALL
                    SELECT @Prefix + 'All FCs' AS GroupName,
                           @Prefix + 'ALLFCS@KOFC-WA.ORG' AS GroupEmail,
                           @Prefix + vo.OfficeDescription + CAST (mm.Assembly AS NVARCHAR) AS RecipientName,
                           @Prefix + vo.EmailAlias + CAST (mm.Assembly AS NVARCHAR) + '@KOFC-WA.ORG' AS RecipientEmail,
                           @ManagedBy AS ManagedBy,
                           vo.EmailAlias,
                           vo.GroupID,
                           vo.OfficeID
                    FROM   tbl_CorrMemberOffice AS cmo
                           INNER JOIN
                           tbl_ValOffices AS vo
                           ON cmo.OfficeID = vo.OfficeID
                           INNER JOIN
                           tbl_MasMembers AS mm
                           ON cmo.MemberID = mm.MemberID
                    WHERE  vo.Officeid IN (17)
                           AND cmo.Year = dbo.funSYS_GetBegFratYearN(0)) AS Results
            WHERE  (@GroupID = 0
                    OR Results.GroupID = @GroupID)
                   AND (@OfficeID = 0
                        OR Results.OfficeID = @OfficeID);
        END
END