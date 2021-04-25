CREATE PROCEDURE sp_CommitPageLinks
@links as tvp_StringTable readonly,
@storeID as uniqueidentifier
as
BEGIN

	INSERT INTO [Page]
	SELECT
		[value] as [url],
		@storeID,
		NULL,
		GETDATE(),
		NULL,
		1,
		0
	FROM 
		@links L
		LEFT JOIN 
		[Page] ON [Page].url = L.[value]
	WHERE
		[Page].url IS NULL


END