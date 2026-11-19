USE [B2BWholesaleA2];
SELECT 'Item' AS TableName, COUNT(*) AS RecordCount FROM dbo.Item
UNION ALL SELECT 'Agent',COUNT(*) FROM dbo.Agent
UNION ALL SELECT 'Order',COUNT(*) FROM dbo.[Order]
UNION ALL SELECT 'OrderDetail',COUNT(*) FROM dbo.OrderDetail
UNION ALL SELECT 'Users',COUNT(*) FROM dbo.Users;
SELECT TOP (10) i.ItemID,i.ItemName,i.StockQuantity,i.Price FROM dbo.Item i ORDER BY i.ItemID;
SELECT TOP (10) o.OrderID,a.AgentName,o.OrderStatus,SUM(d.Quantity*d.UnitAmount) OrderTotal
FROM dbo.[Order] o JOIN dbo.Agent a ON a.AgentID=o.AgentID
JOIN dbo.OrderDetail d ON d.OrderID=o.OrderID
GROUP BY o.OrderID,a.AgentName,o.OrderStatus ORDER BY o.OrderID;
SELECT TOP (10) i.ItemName,SUM(d.Quantity) UnitsSold
FROM dbo.OrderDetail d JOIN dbo.Item i ON i.ItemID=d.ItemID
GROUP BY i.ItemName ORDER BY UnitsSold DESC;
SELECT ItemName,StockQuantity FROM dbo.Item WHERE StockQuantity < 10;
SELECT a.AgentName,COALESCE(SUM(d.Quantity*d.UnitAmount),0) Revenue
FROM dbo.Agent a LEFT JOIN dbo.[Order] o ON o.AgentID=a.AgentID
LEFT JOIN dbo.OrderDetail d ON d.OrderID=o.OrderID
GROUP BY a.AgentName ORDER BY Revenue DESC;
