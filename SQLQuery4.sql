/* Minimal, flat mapping table (adjust names later if you add a richer model) */
IF OBJECT_ID('dbo.InstrumentMapping','U') IS NULL
BEGIN
    CREATE TABLE dbo.InstrumentMapping(
        InstrumentCode  nvarchar(40) NOT NULL,     -- e.g. GLU from analyzer
        LabTestCode     nvarchar(40) NOT NULL,     -- e.g. GLU in your catalog
        IsActive        bit NOT NULL CONSTRAINT DF_InstrumentMapping_IsActive DEFAULT(1),
        CONSTRAINT PK_InstrumentMapping PRIMARY KEY(InstrumentCode)
    );
END;

/* Idempotent seeds */
MERGE dbo.InstrumentMapping AS t
USING (VALUES
    (N'GLU',  N'GLU', 1),
    (N'NA',   N'NA',  1),
    (N'K',    N'K',   1),
    (N'UREA', N'UREA',1)
) AS s(InstrumentCode, LabTestCode, IsActive)
ON  t.InstrumentCode = s.InstrumentCode
WHEN NOT MATCHED BY TARGET THEN
  INSERT(InstrumentCode, LabTestCode, IsActive) VALUES (s.InstrumentCode, s.LabTestCode, s.IsActive)
WHEN MATCHED THEN UPDATE SET LabTestCode=s.LabTestCode, IsActive=s.IsActive;

/* Check */
SELECT * FROM dbo.InstrumentMapping ORDER BY InstrumentCode;
