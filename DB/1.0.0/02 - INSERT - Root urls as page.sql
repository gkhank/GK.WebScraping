use WebScraping

DELETE PAGE
DBCC CHECKIDENT ('Page', RESEED, 0)
INSERT INTO [Page]
select rootUrl, storeID, null, GETDATE(), NULL, 1, 1 FROM Store where name = 'Inet'
