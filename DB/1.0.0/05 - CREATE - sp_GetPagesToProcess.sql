CREATE PROCEDURE sp_GetPagesToProcess
@bulkSize int,
@acceptableReadDate datetime,
@currentMap tinyint, 
@excludingMap as [dbo].[tvp_IntTable] readonly,
--Reading = 1 , Mapping = 2
@orderBy int = 0
AS

	--..\GK.WebScraping\GK.WebScraping.Model\Code\Enums.cs
    --public enum MapStatusType
    --{
    --    None,
    --    ContentInProgress,
    --    ContentReady,
    --    MappingInProgress,
    --    MapReady,
    --    Invalid
    --}

BEGIN
	
	CREATE TABLE #returnIds (value int);
	INSERT INTO #returnIds
	SELECT
        [Page].pageID 
    FROM 
        [Page]

		WITH (XLOCK)
		LEFT JOIN
		@excludingMap [ExcludingMapStatuses]
		ON ([ExcludingMapStatuses].value = [Page].mapStatus)
		INNER JOIN
		[Store] 
		ON [Store].storeID = [Page].storeID
    WHERE
		[ExcludingMapStatuses].value IS NULL
		AND
        [Page].status = 1
        AND
        [Page].deleteDate IS NULL
        AND
        (
			[Page].lastReadDate IS NULL
			OR
			[Page].lastReadDate < @acceptableReadDate
        )
	ORDER BY 
		CASE 
			WHEN @orderBy = 0
			THEN pageID
		END

	OFFSET 0 ROWS 
	FETCH FIRST @bulkSize ROWS ONLY;


	UPDATE [Page]
	SET
	mapStatus = @currentMap
	FROM 
		[Page]
		INNER JOIN
		#returnIds RIDS ON RIDS.value = [Page].pageID


	SELECT 
		[Page].*
	FROM 
		[Page]		
		INNER JOIN
		#returnIds RIDS ON RIDS.value = [Page].pageID
		INNER JOIN
		[Store] ON [Store].storeID = [Page].storeID

	DROP TABLE #returnIds

END
