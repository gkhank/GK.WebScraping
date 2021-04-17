use WebScraping
ALTER TABLE
  [Page]
ALTER COLUMN
  content
    NVARCHAR(max) NULL;

GO
ALTER TABLE
  [Page]
ALTER COLUMN
  url		
    NVARCHAR(850) NULL;