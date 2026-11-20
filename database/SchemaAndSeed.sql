-- Optional SQL Server standalone provisioning script for Assignment 2.
-- For Option C (EF Core Code First) the recommended path is automatic EnsureCreated + DemoSeeder.
-- Run in SSMS only on a clean local TEST SQL Server. Do not run against production data.
IF DB_ID(N'B2BWholesaleA2') IS NULL CREATE DATABASE [B2BWholesaleA2];
GO
USE [B2BWholesaleA2];
GO
IF OBJECT_ID(N'dbo.Item', N'U') IS NOT NULL THROW 50001, 'Target database is not empty; use a new local test database.', 1;
CREATE TABLE dbo.Item (ItemID INT IDENTITY(1,1) PRIMARY KEY, ItemName NVARCHAR(120) NOT NULL,
 [Size] NVARCHAR(30) NOT NULL, Price DECIMAL(18,2) NOT NULL CHECK(Price >= 0),
 StockQuantity INT NOT NULL CHECK(StockQuantity >= 0), CONSTRAINT UQ_Item_Name_Size UNIQUE(ItemName,[Size]));
CREATE TABLE dbo.Agent (AgentID INT IDENTITY(1,1) PRIMARY KEY, AgentName NVARCHAR(120) NOT NULL,
 [Address] NVARCHAR(240) NOT NULL, Phone NVARCHAR(25) NOT NULL);
CREATE TABLE dbo.[Order] (OrderID INT IDENTITY(1,1) PRIMARY KEY, OrderDate DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
 AgentID INT NOT NULL, OrderStatus NVARCHAR(20) NOT NULL DEFAULT 'Pending',
 CONSTRAINT CK_Order_Status CHECK(OrderStatus IN ('Pending','Processing','Shipped','Cancelled')),
 CONSTRAINT FK_Order_Agent FOREIGN KEY(AgentID) REFERENCES dbo.Agent(AgentID));
CREATE TABLE dbo.OrderDetail (ID INT IDENTITY(1,1) PRIMARY KEY, OrderID INT NOT NULL, ItemID INT NOT NULL,
 Quantity INT NOT NULL CHECK(Quantity > 0), UnitAmount DECIMAL(18,2) NOT NULL CHECK(UnitAmount >= 0),
 CONSTRAINT UQ_OrderDetail_Order_Item UNIQUE(OrderID,ItemID),
 CONSTRAINT FK_Detail_Order FOREIGN KEY(OrderID) REFERENCES dbo.[Order](OrderID) ON DELETE CASCADE,
 CONSTRAINT FK_Detail_Item FOREIGN KEY(ItemID) REFERENCES dbo.Item(ItemID));
CREATE TABLE dbo.Users (UserID INT IDENTITY(1,1) PRIMARY KEY, UserName NVARCHAR(60) NOT NULL UNIQUE,
 Email NVARCHAR(160) NOT NULL UNIQUE, PasswordHash NVARCHAR(500) NOT NULL,
 IsLocked BIT NOT NULL DEFAULT 0, FailedLoginCount INT NOT NULL DEFAULT 0,
 [Role] NVARCHAR(10) NOT NULL CHECK([Role] IN ('Admin','Agent')),
 AgentID INT NULL, CONSTRAINT FK_Users_Agent FOREIGN KEY(AgentID) REFERENCES dbo.Agent(AgentID));
-- The hashes below use Microsoft ASP.NET Identity's PBKDF2 V3 format.
-- Demo credentials only; never use these credentials for production.

INSERT dbo.Item(ItemName,[Size],Price,StockQuantity) VALUES(N'A4 Copy Paper',N'Ream 500',78000,120);
INSERT dbo.Item(ItemName,[Size],Price,StockQuantity) VALUES(N'Blue Ballpoint Pen',N'Box 50',155000,80);
INSERT dbo.Item(ItemName,[Size],Price,StockQuantity) VALUES(N'A5 Notebook',N'Pack 12',132000,55);
INSERT dbo.Item(ItemName,[Size],Price,StockQuantity) VALUES(N'Shipping Box Small',N'Pack 20',110000,68);
INSERT dbo.Item(ItemName,[Size],Price,StockQuantity) VALUES(N'Shipping Box Medium',N'Pack 20',165000,48);
INSERT dbo.Item(ItemName,[Size],Price,StockQuantity) VALUES(N'Packing Tape',N'Roll 100m',26000,95);
INSERT dbo.Item(ItemName,[Size],Price,StockQuantity) VALUES(N'USB 32GB',N'Piece',99000,34);
INSERT dbo.Item(ItemName,[Size],Price,StockQuantity) VALUES(N'Wireless Mouse',N'Piece',179000,24);
INSERT dbo.Item(ItemName,[Size],Price,StockQuantity) VALUES(N'Office Keyboard',N'Piece',219000,19);
INSERT dbo.Item(ItemName,[Size],Price,StockQuantity) VALUES(N'A4 File Folder',N'Pack 25',72000,100);
INSERT dbo.Item(ItemName,[Size],Price,StockQuantity) VALUES(N'Whiteboard Marker',N'Box 12',125000,72);
INSERT dbo.Item(ItemName,[Size],Price,StockQuantity) VALUES(N'Receipt Paper',N'Box 20',288000,18);
INSERT dbo.Item(ItemName,[Size],Price,StockQuantity) VALUES(N'Bubble Wrap',N'Roll 50m',175000,7);
INSERT dbo.Item(ItemName,[Size],Price,StockQuantity) VALUES(N'Bubble Mailer',N'Pack 100',260000,15);
INSERT dbo.Item(ItemName,[Size],Price,StockQuantity) VALUES(N'Safety Gloves',N'Box 100',98000,5);
INSERT dbo.Item(ItemName,[Size],Price,StockQuantity) VALUES(N'Shipping Label',N'Roll 500',138000,62);
INSERT dbo.Item(ItemName,[Size],Price,StockQuantity) VALUES(N'Heavy Duty Stapler',N'Piece',249000,12);
INSERT dbo.Item(ItemName,[Size],Price,StockQuantity) VALUES(N'Black Printer Toner',N'Cartridge',425000,9);
INSERT dbo.Item(ItemName,[Size],Price,StockQuantity) VALUES(N'Desk Organizer',N'Piece',92000,33);
INSERT dbo.Item(ItemName,[Size],Price,StockQuantity) VALUES(N'Microfiber Cloth',N'Pack 20',85000,44);
INSERT dbo.Agent(AgentName,[Address],Phone) VALUES(N'Wholesale Partner 01',N'11 Demo Street, District 2, Ho Chi Minh City',N'0901000001');
INSERT dbo.Agent(AgentName,[Address],Phone) VALUES(N'Wholesale Partner 02',N'12 Demo Street, District 3, Ho Chi Minh City',N'0901000002');
INSERT dbo.Agent(AgentName,[Address],Phone) VALUES(N'Wholesale Partner 03',N'13 Demo Street, District 4, Ho Chi Minh City',N'0901000003');
INSERT dbo.Agent(AgentName,[Address],Phone) VALUES(N'Wholesale Partner 04',N'14 Demo Street, District 5, Ho Chi Minh City',N'0901000004');
INSERT dbo.Agent(AgentName,[Address],Phone) VALUES(N'Wholesale Partner 05',N'15 Demo Street, District 6, Ho Chi Minh City',N'0901000005');
INSERT dbo.Agent(AgentName,[Address],Phone) VALUES(N'Wholesale Partner 06',N'16 Demo Street, District 7, Ho Chi Minh City',N'0901000006');
INSERT dbo.Agent(AgentName,[Address],Phone) VALUES(N'Wholesale Partner 07',N'17 Demo Street, District 8, Ho Chi Minh City',N'0901000007');
INSERT dbo.Agent(AgentName,[Address],Phone) VALUES(N'Wholesale Partner 08',N'18 Demo Street, District 9, Ho Chi Minh City',N'0901000008');
INSERT dbo.Agent(AgentName,[Address],Phone) VALUES(N'Wholesale Partner 09',N'19 Demo Street, District 10, Ho Chi Minh City',N'0901000009');
INSERT dbo.Agent(AgentName,[Address],Phone) VALUES(N'Wholesale Partner 10',N'20 Demo Street, District 1, Ho Chi Minh City',N'0901000010');
INSERT dbo.Agent(AgentName,[Address],Phone) VALUES(N'Wholesale Partner 11',N'21 Demo Street, District 2, Ho Chi Minh City',N'0901000011');
INSERT dbo.Agent(AgentName,[Address],Phone) VALUES(N'Wholesale Partner 12',N'22 Demo Street, District 3, Ho Chi Minh City',N'0901000012');
INSERT dbo.Agent(AgentName,[Address],Phone) VALUES(N'Wholesale Partner 13',N'23 Demo Street, District 4, Ho Chi Minh City',N'0901000013');
INSERT dbo.Agent(AgentName,[Address],Phone) VALUES(N'Wholesale Partner 14',N'24 Demo Street, District 5, Ho Chi Minh City',N'0901000014');
INSERT dbo.Agent(AgentName,[Address],Phone) VALUES(N'Wholesale Partner 15',N'25 Demo Street, District 6, Ho Chi Minh City',N'0901000015');
INSERT dbo.Agent(AgentName,[Address],Phone) VALUES(N'Wholesale Partner 16',N'26 Demo Street, District 7, Ho Chi Minh City',N'0901000016');
INSERT dbo.Agent(AgentName,[Address],Phone) VALUES(N'Wholesale Partner 17',N'27 Demo Street, District 8, Ho Chi Minh City',N'0901000017');
INSERT dbo.Agent(AgentName,[Address],Phone) VALUES(N'Wholesale Partner 18',N'28 Demo Street, District 9, Ho Chi Minh City',N'0901000018');
INSERT dbo.Agent(AgentName,[Address],Phone) VALUES(N'Wholesale Partner 19',N'29 Demo Street, District 10, Ho Chi Minh City',N'0901000019');
INSERT dbo.Agent(AgentName,[Address],Phone) VALUES(N'Wholesale Partner 20',N'30 Demo Street, District 1, Ho Chi Minh City',N'0901000020');
INSERT dbo.Users(UserName,Email,PasswordHash,IsLocked,FailedLoginCount,[Role],AgentID) VALUES(N'admin',N'admin@example.test',N'AQAAAAIAAYagAAAAEIxpduW1QQQVvekIvU3uFd9Lf89ZeMO6bsFHAhaX47b6zWvEo2v7Svsg3+DAH9M7HQ==',0,0,N'Admin',NULL);
INSERT dbo.Users(UserName,Email,PasswordHash,IsLocked,FailedLoginCount,[Role],AgentID) VALUES(N'admin02',N'admin02@example.test',N'AQAAAAIAAYagAAAAECQ6Wk9OOifehjkfB88FintdHMj5+ig4VWq8Y2vV1PGZ8NAm7ZxuOBMRpQj5O7rBMg==',0,0,N'Admin',NULL);
INSERT dbo.Users(UserName,Email,PasswordHash,IsLocked,FailedLoginCount,[Role],AgentID) VALUES(N'agent01',N'agent01@example.test',N'AQAAAAIAAYagAAAAEDpfVgJxsc3JsCkbeXbuICKep3WbIdmoLx8TNrtLqQpuXOefwValhz8CfBAhOJuazg==',0,0,N'Agent',1);
INSERT dbo.Users(UserName,Email,PasswordHash,IsLocked,FailedLoginCount,[Role],AgentID) VALUES(N'agent02',N'agent02@example.test',N'AQAAAAIAAYagAAAAENcdKRafjN++dAbHCjnIuccgLO2d/0WZxs2j9U290WW5ED64PJUUUhux2VdT6yMd4A==',0,0,N'Agent',2);
INSERT dbo.Users(UserName,Email,PasswordHash,IsLocked,FailedLoginCount,[Role],AgentID) VALUES(N'agent03',N'agent03@example.test',N'AQAAAAIAAYagAAAAEDc8sk18vD38RC+TtnBsIC7PQggZRYntOrPxGsqhadz0+NaQ7fTnLZjGRAqGPiSteA==',0,0,N'Agent',3);
INSERT dbo.Users(UserName,Email,PasswordHash,IsLocked,FailedLoginCount,[Role],AgentID) VALUES(N'agent04',N'agent04@example.test',N'AQAAAAIAAYagAAAAEO5tcQHu/fq0T3/pQVVQUr+PsAHtsETNyc/FCUgRrn1zGC3oHoK5nOz8xSPG3vF2zQ==',0,0,N'Agent',4);
INSERT dbo.Users(UserName,Email,PasswordHash,IsLocked,FailedLoginCount,[Role],AgentID) VALUES(N'agent05',N'agent05@example.test',N'AQAAAAIAAYagAAAAEIV3AL8WVJADErgMWEghTrEi77BFcxIHctAvPZoV+pifXd7CsjZSzfQpg62mmtpHbg==',0,0,N'Agent',5);
INSERT dbo.Users(UserName,Email,PasswordHash,IsLocked,FailedLoginCount,[Role],AgentID) VALUES(N'agent06',N'agent06@example.test',N'AQAAAAIAAYagAAAAEI5YimMRcfm4sV8nuaDbb0psOERWtjJhEeeAO3TZS3upMvuXYk5slHKsrpIW06JvJA==',0,0,N'Agent',6);
INSERT dbo.Users(UserName,Email,PasswordHash,IsLocked,FailedLoginCount,[Role],AgentID) VALUES(N'agent07',N'agent07@example.test',N'AQAAAAIAAYagAAAAEIA8JIJq2DR9E9QoCriQzhRS+/2++zgpGWKJpePtx0hZBbevTQ1NyNl3zNKi2c9Njw==',0,0,N'Agent',7);
INSERT dbo.Users(UserName,Email,PasswordHash,IsLocked,FailedLoginCount,[Role],AgentID) VALUES(N'agent08',N'agent08@example.test',N'AQAAAAIAAYagAAAAEMFi+pMlFXj++AI4HQ0bTNmn0UYvznQr5/aoVsznASDzJSjGSXzfmbM8+NszldsCuA==',0,0,N'Agent',8);
INSERT dbo.Users(UserName,Email,PasswordHash,IsLocked,FailedLoginCount,[Role],AgentID) VALUES(N'agent09',N'agent09@example.test',N'AQAAAAIAAYagAAAAEMN7Lfa8D3K7kSGO8qW6dPHDITSEp51O9c9M+LYTBv7u1Lna2BE87uHdlD1RWshUyQ==',0,0,N'Agent',9);
INSERT dbo.Users(UserName,Email,PasswordHash,IsLocked,FailedLoginCount,[Role],AgentID) VALUES(N'agent10',N'agent10@example.test',N'AQAAAAIAAYagAAAAEM+8KiONOfSKs2/ei4mVqaZ+FvWIhGnYR4SI7RI6vndjreEtub5g9/TFP3tVSXMi8Q==',0,0,N'Agent',10);
INSERT dbo.Users(UserName,Email,PasswordHash,IsLocked,FailedLoginCount,[Role],AgentID) VALUES(N'agent11',N'agent11@example.test',N'AQAAAAIAAYagAAAAEJUF3837hvV9gXmZscZByMdNPtkL7Ak1ype286BvgnOXy1WD01Zaz81xO8HnXDmIvg==',0,0,N'Agent',11);
INSERT dbo.Users(UserName,Email,PasswordHash,IsLocked,FailedLoginCount,[Role],AgentID) VALUES(N'agent12',N'agent12@example.test',N'AQAAAAIAAYagAAAAEByAuFpr6yIK1cSvvS/Plcai9pg3CQH+IXHoABbNSjuEapYeUY8aXKDpYNZZoHrGtA==',0,0,N'Agent',12);
INSERT dbo.Users(UserName,Email,PasswordHash,IsLocked,FailedLoginCount,[Role],AgentID) VALUES(N'agent13',N'agent13@example.test',N'AQAAAAIAAYagAAAAEDop6uZYT4djekBog5QKZpn4k9z2dYfPvz0rmYzZxYKindFdPonmUgh0AFfgvbMSwA==',0,0,N'Agent',13);
INSERT dbo.Users(UserName,Email,PasswordHash,IsLocked,FailedLoginCount,[Role],AgentID) VALUES(N'agent14',N'agent14@example.test',N'AQAAAAIAAYagAAAAEEJxVWw9nE1KI1wXCVfV4F7hyh5G1867uc0wr9iOLlzIj35gXSvctz13cpcWt+SGxA==',0,0,N'Agent',14);
INSERT dbo.Users(UserName,Email,PasswordHash,IsLocked,FailedLoginCount,[Role],AgentID) VALUES(N'agent15',N'agent15@example.test',N'AQAAAAIAAYagAAAAEHlrWImtPV8Z121XY5eqhIeldTI0WCst/kyp3iX2l1qSLxJ5OOWN7iLk9WBTIGd8og==',0,0,N'Agent',15);
INSERT dbo.Users(UserName,Email,PasswordHash,IsLocked,FailedLoginCount,[Role],AgentID) VALUES(N'agent16',N'agent16@example.test',N'AQAAAAIAAYagAAAAEN2uxD//fxUnX+4hkVrxvzib6UJgQCAOraETXc+Fka5zFGU75VCEsZ/rGltNYGuYFg==',0,0,N'Agent',16);
INSERT dbo.Users(UserName,Email,PasswordHash,IsLocked,FailedLoginCount,[Role],AgentID) VALUES(N'agent17',N'agent17@example.test',N'AQAAAAIAAYagAAAAEH95FVmpwnGXXC8uuc0aICm64IgRCBB5fmZEEYwyhwTtYKMI8OJRRqFn7NqnC6z3qQ==',0,0,N'Agent',17);
INSERT dbo.Users(UserName,Email,PasswordHash,IsLocked,FailedLoginCount,[Role],AgentID) VALUES(N'agent18',N'agent18@example.test',N'AQAAAAIAAYagAAAAEEXsqQw1z1qAErfNvu2a9gW3y4eKJBvSfAHAsUsR9SZV3Ei+8trQHyEweqKU9mLJaw==',0,0,N'Agent',18);
INSERT dbo.[Order](OrderDate,AgentID,OrderStatus) VALUES(DATEADD(day,-1,SYSUTCDATETIME()),1,N'Processing');
INSERT dbo.[Order](OrderDate,AgentID,OrderStatus) VALUES(DATEADD(day,-2,SYSUTCDATETIME()),2,N'Shipped');
INSERT dbo.[Order](OrderDate,AgentID,OrderStatus) VALUES(DATEADD(day,-3,SYSUTCDATETIME()),3,N'Shipped');
INSERT dbo.[Order](OrderDate,AgentID,OrderStatus) VALUES(DATEADD(day,-4,SYSUTCDATETIME()),4,N'Cancelled');
INSERT dbo.[Order](OrderDate,AgentID,OrderStatus) VALUES(DATEADD(day,-5,SYSUTCDATETIME()),5,N'Pending');
INSERT dbo.[Order](OrderDate,AgentID,OrderStatus) VALUES(DATEADD(day,-6,SYSUTCDATETIME()),6,N'Processing');
INSERT dbo.[Order](OrderDate,AgentID,OrderStatus) VALUES(DATEADD(day,-7,SYSUTCDATETIME()),7,N'Shipped');
INSERT dbo.[Order](OrderDate,AgentID,OrderStatus) VALUES(DATEADD(day,-8,SYSUTCDATETIME()),8,N'Shipped');
INSERT dbo.[Order](OrderDate,AgentID,OrderStatus) VALUES(DATEADD(day,-9,SYSUTCDATETIME()),9,N'Cancelled');
INSERT dbo.[Order](OrderDate,AgentID,OrderStatus) VALUES(DATEADD(day,-10,SYSUTCDATETIME()),10,N'Pending');
INSERT dbo.[Order](OrderDate,AgentID,OrderStatus) VALUES(DATEADD(day,-11,SYSUTCDATETIME()),11,N'Processing');
INSERT dbo.[Order](OrderDate,AgentID,OrderStatus) VALUES(DATEADD(day,-12,SYSUTCDATETIME()),12,N'Shipped');
INSERT dbo.[Order](OrderDate,AgentID,OrderStatus) VALUES(DATEADD(day,-13,SYSUTCDATETIME()),13,N'Shipped');
INSERT dbo.[Order](OrderDate,AgentID,OrderStatus) VALUES(DATEADD(day,-14,SYSUTCDATETIME()),14,N'Cancelled');
INSERT dbo.[Order](OrderDate,AgentID,OrderStatus) VALUES(DATEADD(day,-15,SYSUTCDATETIME()),15,N'Pending');
INSERT dbo.[Order](OrderDate,AgentID,OrderStatus) VALUES(DATEADD(day,-16,SYSUTCDATETIME()),16,N'Processing');
INSERT dbo.[Order](OrderDate,AgentID,OrderStatus) VALUES(DATEADD(day,-17,SYSUTCDATETIME()),17,N'Shipped');
INSERT dbo.[Order](OrderDate,AgentID,OrderStatus) VALUES(DATEADD(day,-18,SYSUTCDATETIME()),18,N'Shipped');
INSERT dbo.[Order](OrderDate,AgentID,OrderStatus) VALUES(DATEADD(day,-19,SYSUTCDATETIME()),1,N'Cancelled');
INSERT dbo.[Order](OrderDate,AgentID,OrderStatus) VALUES(DATEADD(day,-20,SYSUTCDATETIME()),2,N'Pending');
INSERT dbo.OrderDetail(OrderID,ItemID,Quantity,UnitAmount) SELECT 1,3,2,Price FROM dbo.Item WHERE ItemID=3;
INSERT dbo.OrderDetail(OrderID,ItemID,Quantity,UnitAmount) SELECT 2,6,3,Price FROM dbo.Item WHERE ItemID=6;
INSERT dbo.OrderDetail(OrderID,ItemID,Quantity,UnitAmount) SELECT 3,9,4,Price FROM dbo.Item WHERE ItemID=9;
INSERT dbo.OrderDetail(OrderID,ItemID,Quantity,UnitAmount) SELECT 4,12,5,Price FROM dbo.Item WHERE ItemID=12;
INSERT dbo.OrderDetail(OrderID,ItemID,Quantity,UnitAmount) SELECT 5,15,6,Price FROM dbo.Item WHERE ItemID=15;
INSERT dbo.OrderDetail(OrderID,ItemID,Quantity,UnitAmount) SELECT 6,18,1,Price FROM dbo.Item WHERE ItemID=18;
INSERT dbo.OrderDetail(OrderID,ItemID,Quantity,UnitAmount) SELECT 7,1,2,Price FROM dbo.Item WHERE ItemID=1;
INSERT dbo.OrderDetail(OrderID,ItemID,Quantity,UnitAmount) SELECT 8,4,3,Price FROM dbo.Item WHERE ItemID=4;
INSERT dbo.OrderDetail(OrderID,ItemID,Quantity,UnitAmount) SELECT 9,7,4,Price FROM dbo.Item WHERE ItemID=7;
INSERT dbo.OrderDetail(OrderID,ItemID,Quantity,UnitAmount) SELECT 10,10,5,Price FROM dbo.Item WHERE ItemID=10;
INSERT dbo.OrderDetail(OrderID,ItemID,Quantity,UnitAmount) SELECT 11,13,6,Price FROM dbo.Item WHERE ItemID=13;
INSERT dbo.OrderDetail(OrderID,ItemID,Quantity,UnitAmount) SELECT 12,16,1,Price FROM dbo.Item WHERE ItemID=16;
INSERT dbo.OrderDetail(OrderID,ItemID,Quantity,UnitAmount) SELECT 13,19,2,Price FROM dbo.Item WHERE ItemID=19;
INSERT dbo.OrderDetail(OrderID,ItemID,Quantity,UnitAmount) SELECT 14,2,3,Price FROM dbo.Item WHERE ItemID=2;
INSERT dbo.OrderDetail(OrderID,ItemID,Quantity,UnitAmount) SELECT 15,5,4,Price FROM dbo.Item WHERE ItemID=5;
INSERT dbo.OrderDetail(OrderID,ItemID,Quantity,UnitAmount) SELECT 16,8,5,Price FROM dbo.Item WHERE ItemID=8;
INSERT dbo.OrderDetail(OrderID,ItemID,Quantity,UnitAmount) SELECT 17,11,6,Price FROM dbo.Item WHERE ItemID=11;
INSERT dbo.OrderDetail(OrderID,ItemID,Quantity,UnitAmount) SELECT 18,14,1,Price FROM dbo.Item WHERE ItemID=14;
INSERT dbo.OrderDetail(OrderID,ItemID,Quantity,UnitAmount) SELECT 19,17,2,Price FROM dbo.Item WHERE ItemID=17;
INSERT dbo.OrderDetail(OrderID,ItemID,Quantity,UnitAmount) SELECT 20,20,3,Price FROM dbo.Item WHERE ItemID=20;
INSERT dbo.OrderDetail(OrderID,ItemID,Quantity,UnitAmount) SELECT 1,12,2,Price FROM dbo.Item WHERE ItemID=12;
INSERT dbo.OrderDetail(OrderID,ItemID,Quantity,UnitAmount) SELECT 2,19,3,Price FROM dbo.Item WHERE ItemID=19;
INSERT dbo.OrderDetail(OrderID,ItemID,Quantity,UnitAmount) SELECT 3,6,1,Price FROM dbo.Item WHERE ItemID=6;
INSERT dbo.OrderDetail(OrderID,ItemID,Quantity,UnitAmount) SELECT 4,13,2,Price FROM dbo.Item WHERE ItemID=13;
INSERT dbo.OrderDetail(OrderID,ItemID,Quantity,UnitAmount) SELECT 5,20,3,Price FROM dbo.Item WHERE ItemID=20;
INSERT dbo.OrderDetail(OrderID,ItemID,Quantity,UnitAmount) SELECT 6,7,1,Price FROM dbo.Item WHERE ItemID=7;
INSERT dbo.OrderDetail(OrderID,ItemID,Quantity,UnitAmount) SELECT 7,14,2,Price FROM dbo.Item WHERE ItemID=14;
INSERT dbo.OrderDetail(OrderID,ItemID,Quantity,UnitAmount) SELECT 8,1,3,Price FROM dbo.Item WHERE ItemID=1;
SELECT 'Item' AS Entity, COUNT(*) AS Rows FROM dbo.Item UNION ALL SELECT 'Agent', COUNT(*) FROM dbo.Agent UNION ALL SELECT 'Order', COUNT(*) FROM dbo.[Order] UNION ALL SELECT 'OrderDetail', COUNT(*) FROM dbo.OrderDetail UNION ALL SELECT 'Users', COUNT(*) FROM dbo.Users;
