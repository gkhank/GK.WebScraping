use WebScraping

delete [Page]
INSERT INTO [Page]
select rootUrl, storeID, null, GETDATE(), NULL, 1, 1 FROM Store where name = 'Amazon.se'
