INSERT INTO "Apartments" ("Id", "Title", "Description", "Location", "Price", "ExternalId", "Metadata")
VALUES (@Id, @Title, @Description, @Location, @Price, @ExternalId, @Metadata::jsonb)
ON CONFLICT ("Id") DO UPDATE SET
    "Title" = EXCLUDED."Title",
    "Description" = EXCLUDED."Description",
    "Location" = EXCLUDED."Location",
    "Price" = EXCLUDED."Price",
    "ExternalId" = EXCLUDED."ExternalId",
    "Metadata" = EXCLUDED."Metadata";