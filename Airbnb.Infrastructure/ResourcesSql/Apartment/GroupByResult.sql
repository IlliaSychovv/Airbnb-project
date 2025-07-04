SELECT "Location",
       COUNT(*) AS ApartmentCount,
       AVG("Price") AS AvgPrice
FROM "Apartments"
GROUP BY "Location";