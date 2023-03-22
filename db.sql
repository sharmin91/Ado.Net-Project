CREATE Table publishers
(
	publisherid INT PRIMARY KEY,
	publishername NVARCHAR(50) NOT NULL
)
GO
CREATE Table categories
(
	categoryid INT PRIMARY KEY,
	categoryname NVARCHAR(50) NOT NULL
)
GO
CREATE TABLE books
(
	bookid INT PRIMARY KEY,
	title NVARCHAR(50) NOT NULL,
	publishdate DATE NOT NULL,
	price MONEY NOT NULL,
	available BIT ,
	coverpage NVARCHAR(150) NOT NULL,
	publisherid INT NOT NULL REFERENCES publishers(publisherid),
	categoryid INT NOT NULL REFERENCES categories(categoryid),
)
GO
CREATE TABLE TOCs
(
	tocid INT PRIMARY KEY,
	bookid INT NOT NULL REFERENCES books(bookid),
	chapterno INT NOT NULL,
	chaptertitle NVARCHAR(100) NOT NULL,
	totalpages INT NOT NULL
)
GO