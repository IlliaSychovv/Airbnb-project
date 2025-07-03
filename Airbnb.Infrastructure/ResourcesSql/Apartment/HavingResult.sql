SELECT Location,
       COUNT(*) AS ApartmentCount,
       AVG(Price) AS AvgPrice
FROM Apartment
GROUP BY Location
HAVING AVG(Price) > 500;