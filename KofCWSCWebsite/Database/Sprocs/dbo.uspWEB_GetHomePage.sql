alter PROCEDURE [dbo].[uspWEB_GetHomePage]
AS
BEGIN
    CREATE TABLE #HomeData (
        Photo             VARCHAR (250),
        Email             VARCHAR (250),
        FullName          VARCHAR (250),
        OfficeDescription VARCHAR (250),
        TagLine           VARCHAR (250),
        Data              VARCHAR (MAX),
        class             VARCHAR (50) ,
        SortOrder         INT          ,
        URL               VARCHAR (250),
        OID               INT          ,
        Type              VARCHAR (2)  ,
        Title             VARCHAR (250),
        GraphicURL        VARCHAR (250),
        LinkURL           VARCHAR (MAX),
        PostedDate        DATETIME     ,
        Expired           BIT          
    );
    INSERT INTO #HomeData
    EXECUTE [uspWEB_GetSOS] 0;
    INSERT INTO #HomeData
    SELECT '' AS Photo,
           '' AS Email,
           '' AS FullName,
           '' AS OfficeDescription,
           '' AS TagLine,
           Text AS Data,
           '' AS class,
           0 AS SortOrder,
           '' AS URL,
           0 AS OID,
           [Type] AS [Type],
           Title,
           GraphicURL,
           LinkURL,
           PostedDate,
           Expired
    FROM   tblWEB_TrxAOI
    WHERE  Expired <> 1;
    INSERT INTO #HomeData
    SELECT '' AS Photo,
           '' AS Email,
           '' AS FullName,
           '' AS OfficeDescription,
           '' AS TagLine,
           '' AS Data,
           '' AS class,
           0 AS SortOrder,
           '' AS URL,
           0 AS OID,
           'CO' AS [Type],
           '' AS Title,
           '' AS GraphicURL,
           '' AS LinkURL,
           '' AS PostedDate,
           0 AS Expired
    FROM   [tblLOG_CorrMemberOffice] AS cmol
           LEFT OUTER JOIN
           tbl_ValOffices AS vo
           ON cmol.OfficeID = vo.OfficeID
    WHERE  Processed = 0
           AND vo.ExchangeMailType = 'DistributionList';
    INSERT INTO #HomeData
    SELECT top(15)
	'' as Photo,
	'' as Email,
	'' as FullName,
	'' as OfficeDescription,
    CONVERT(VARCHAR(10), CONVERT(DATE,StartDateTime,101), 120) as TagLine,
	--FORMAT(StartDateTime,'MM/dd/yyyy') as TagLine,
	Description as Data,
	'' as Class,
	0 as SortOrder,
	'' as URL,
	0 as OID,
	'CA' as Type,
	Title as Title,
	'' as GraphicURL,
	'' as LinkUrl,
	GETDATE() as PostedDate,
	0 as Expired
FROM tblCAL_trxEvents
WHERE StartDateTime > GETDATE()
ORDER BY StartDateTime ASC
    SELECT   *
    FROM     #HomeData
    ORDER BY PostedDate DESC;
END