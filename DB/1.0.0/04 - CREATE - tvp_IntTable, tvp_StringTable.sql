USE WebScraping
GO

CREATE TYPE tvp_IntTable AS TABLE 
(
	[value] int null
)
GO
CREATE TYPE tvp_StringTable AS TABLE 
(
	[value] varchar(max) null
)
GO

