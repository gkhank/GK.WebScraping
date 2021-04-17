USE WebScraping;   
GO  
ALTER TABLE [Page]   
ADD CONSTRAINT UQ_PageUrl UNIQUE ([url]);   
GO  