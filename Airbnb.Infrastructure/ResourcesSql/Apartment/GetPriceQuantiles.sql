SELECT 
    percentile_cont(0.25) WITHIN GROUP (ORDER BY "Price") AS q1,
    percentile_cont(0.5) WITHIN GROUP (ORDER BY "Price") AS median,
    percentile_cont(0.75) WITHIN GROUP (ORDER BY "Price") AS q3   
FROM "Apartments";