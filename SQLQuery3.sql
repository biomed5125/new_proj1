/* ===== Sequence (used by Comm tables) ===== */
IF NOT EXISTS (SELECT 1 FROM sys.sequences WHERE name='HmsIdSeq_Comm' AND schema_id = SCHEMA_ID('dbo'))
    CREATE SEQUENCE dbo.HmsIdSeq_Comm AS bigint START WITH 2025900000000 INCREMENT BY 1 NO CACHE;

/* ===== AnalyzerProfiles (explicit IDs so they match your screenshots) ===== */
MERGE dbo.AnalyzerProfiles AS t
USING (VALUES
    (CAST(2025900000000 AS bigint), N'Roche Cobas e411', N'ASTM', N'RocheCobasDriver', N'{"Baud":9600,"Parity":0,"DataBits":8,"StopBits":1}', N'Cobas',   N'Elecsys'),
    (CAST(2025900000001 AS bigint), N'Roche Cobas c311', N'ASTM', N'RocheCobasDriver', N'{"Baud":9600,"Parity":0,"DataBits":8,"StopBits":1}', N'Cobas',   N'Chemistry'),
    (CAST(2025900000002 AS bigint), N'Sysmex XP-300',    N'SUIT', N'SysmexSuitDriver', N'{"Baud":9600,"Parity":0,"DataBits":8,"StopBits":1}', NULL,       N'Implements SUIT v8'),
    (CAST(2025900000003 AS bigint), N'Fuji Dri-Chem NX500', N'ASCII', N'FujiDryChemDriver', N'{"Baud":9600,"Parity":0,"DataBits":8,"StopBits":1}', NULL, N'Simple ASCII text frame')
) AS s(AnalyzerProfileId, Name, Protocol, DriverClass, PortSettings, DefaultMode, Notes)
ON  t.AnalyzerProfileId = s.AnalyzerProfileId
WHEN NOT MATCHED BY TARGET THEN
  INSERT (AnalyzerProfileId, Name, Protocol, DriverClass, PortSettings, DefaultMode, Notes)
  VALUES (s.AnalyzerProfileId, s.Name, s.Protocol, s.DriverClass, s.PortSettings, s.DefaultMode, s.Notes)
WHEN MATCHED THEN
  UPDATE SET Name=s.Name, Protocol=s.Protocol, DriverClass=s.DriverClass, PortSettings=s.PortSettings, DefaultMode=s.DefaultMode, Notes=s.Notes;

/* ===== CommDevices (use your two device IDs 2025900000004/5) ===== */
MERGE dbo.CommDevices AS t
USING (VALUES
    (CAST(2025900000004 AS bigint), N'Cobas e411 (Immuno)', N'COBAS-E411-01', N'COM5', CAST(2025900000000 AS bigint), 1),
    (CAST(2025900000005 AS bigint), N'Cobas e411 (Immuno)', N'ROCHE1',        N'COM5', CAST(2025900000000 AS bigint), 1)
) AS s(CommDeviceId, Name, DeviceCode, PortName, AnalyzerProfileId, IsActive)
ON  t.CommDeviceId = s.CommDeviceId
WHEN NOT MATCHED BY TARGET THEN
  INSERT (CommDeviceId, Name, DeviceCode, PortName, AnalyzerProfileId, IsActive)
  VALUES (s.CommDeviceId, s.Name, s.DeviceCode, s.PortName, s.AnalyzerProfileId, s.IsActive)
WHEN MATCHED THEN
  UPDATE SET Name=s.Name, DeviceCode=s.DeviceCode, PortName=s.PortName, AnalyzerProfileId=s.AnalyzerProfileId, IsActive=s.IsActive;

/* Also guarantee DeviceCode uniqueness (helpful) */
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name='IX_CommDevices_DeviceCode' AND object_id=OBJECT_ID('dbo.CommDevices'))
    CREATE UNIQUE INDEX IX_CommDevices_DeviceCode ON dbo.CommDevices(DeviceCode);

/* ===== RouterRule: send all ASTM Result records (R) to LabResult ===== */
MERGE dbo.RouterRule AS t
USING (VALUES
    (CAST(2025900000006 AS bigint), 1, NULL, N'R', N'*', N'LabResult', 10, 0)
) AS s(Id, IsEnabled, DeviceId, RecordType, TestCodeRegex, Target, Priority, IsDeleted)
ON  t.Id = s.Id
WHEN NOT MATCHED BY TARGET THEN
  INSERT (Id, IsEnabled, DeviceId, RecordType, TestCodeRegex, Target, Priority, IsDeleted)
  VALUES (s.Id, s.IsEnabled, s.DeviceId, s.RecordType, s.TestCodeRegex, s.Target, s.Priority, s.IsDeleted)
WHEN MATCHED THEN
  UPDATE SET IsEnabled=s.IsEnabled, DeviceId=s.DeviceId, RecordType=s.RecordType, TestCodeRegex=s.TestCodeRegex, Target=s.Target, Priority=s.Priority, IsDeleted=s.IsDeleted;

/* Quick checks */
SELECT TOP 10 * FROM dbo.AnalyzerProfiles ORDER BY AnalyzerProfileId;
SELECT TOP 10 * FROM dbo.CommDevices ORDER BY CommDeviceId;
SELECT TOP 10 * FROM dbo.RouterRule ORDER BY Id;
