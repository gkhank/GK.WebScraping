use WebScraping

DELETE PAGE
DBCC CHECKIDENT ('Page', RESEED, 0)
INSERT INTO [Page]
select rootUrl, storeID, null, GETDATE(), NULL, 1, 0 FROM Store where name = 'Amazon.se'



