use WebScraping
INSERT INTO [Page]
select NEWID(), storeID, rootUrl, null, null, GETDATE(), NULL, 0, 0 FROM Store