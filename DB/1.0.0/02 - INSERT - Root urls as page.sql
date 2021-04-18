use WebScraping
INSERT INTO [Page]
select NEWID(), storeID, rootUrl, null, null, GETDATE(), NULL, 1, 1 FROM Store