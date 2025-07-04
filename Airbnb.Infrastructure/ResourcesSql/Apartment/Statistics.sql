SELECT
    COUNT(*) AS Count,
     AVG("Price") AS AvgPrice,
      SUM("Price") AS SumPrice,
        MAX("Price") As MaxPrice
FROM "Apartments";