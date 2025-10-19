USE [HMS_Lab];
GO

-- Helper: add a default if the column exists and has no default yet
-- IsDeleted -> 0, CreatedAt -> SYSUTCDATETIME()

-- SpecimenTypes
IF COL_LENGTH('dbo.SpecimenTypes','IsDeleted') IS NOT NULL
AND NOT EXISTS (SELECT 1 FROM sys.default_constraints dc
  JOIN sys.columns c ON c.default_object_id = dc.object_id
  WHERE dc.parent_object_id = OBJECT_ID('dbo.SpecimenTypes') AND c.name = 'IsDeleted')
ALTER TABLE dbo.SpecimenTypes ADD CONSTRAINT DF_SpecimenTypes_IsDeleted DEFAULT(0) FOR IsDeleted;

IF COL_LENGTH('dbo.SpecimenTypes','CreatedAt') IS NOT NULL
AND NOT EXISTS (SELECT 1 FROM sys.default_constraints dc
  JOIN sys.columns c ON c.default_object_id = dc.object_id
  WHERE dc.parent_object_id = OBJECT_ID('dbo.SpecimenTypes') AND c.name = 'CreatedAt')
ALTER TABLE dbo.SpecimenTypes ADD CONSTRAINT DF_SpecimenTypes_CreatedAt DEFAULT (SYSUTCDATETIME()) FOR CreatedAt;

-- ReferenceRanges
IF COL_LENGTH('dbo.ReferenceRanges','IsDeleted') IS NOT NULL
AND NOT EXISTS (SELECT 1 FROM sys.default_constraints dc
  JOIN sys.columns c ON c.default_object_id = dc.object_id
  WHERE dc.parent_object_id = OBJECT_ID('dbo.ReferenceRanges') AND c.name = 'IsDeleted')
ALTER TABLE dbo.ReferenceRanges ADD CONSTRAINT DF_ReferenceRanges_IsDeleted DEFAULT(0) FOR IsDeleted;

IF COL_LENGTH('dbo.ReferenceRanges','CreatedAt') IS NOT NULL
AND NOT EXISTS (SELECT 1 FROM sys.default_constraints dc
  JOIN sys.columns c ON c.default_object_id = dc.object_id
  WHERE dc.parent_object_id = OBJECT_ID('dbo.ReferenceRanges') AND c.name = 'CreatedAt')
ALTER TABLE dbo.ReferenceRanges ADD CONSTRAINT DF_ReferenceRanges_CreatedAt DEFAULT (SYSUTCDATETIME()) FOR CreatedAt;

-- LabTests
IF COL_LENGTH('dbo.LabTests','IsDeleted') IS NOT NULL
AND NOT EXISTS (SELECT 1 FROM sys.default_constraints dc
  JOIN sys.columns c ON c.default_object_id = dc.object_id
  WHERE dc.parent_object_id = OBJECT_ID('dbo.LabTests') AND c.name = 'IsDeleted')
ALTER TABLE dbo.LabTests ADD CONSTRAINT DF_LabTests_IsDeleted DEFAULT(0) FOR IsDeleted;

IF COL_LENGTH('dbo.LabTests','CreatedAt') IS NOT NULL
AND NOT EXISTS (SELECT 1 FROM sys.default_constraints dc
  JOIN sys.columns c ON c.default_object_id = dc.object_id
  WHERE dc.parent_object_id = OBJECT_ID('dbo.LabTests') AND c.name = 'CreatedAt')
ALTER TABLE dbo.LabTests ADD CONSTRAINT DF_LabTests_CreatedAt DEFAULT (SYSUTCDATETIME()) FOR CreatedAt;

-- LabPanels
IF COL_LENGTH('dbo.LabPanels','IsDeleted') IS NOT NULL
AND NOT EXISTS (SELECT 1 FROM sys.default_constraints dc
  JOIN sys.columns c ON c.default_object_id = dc.object_id
  WHERE dc.parent_object_id = OBJECT_ID('dbo.LabPanels') AND c.name = 'IsDeleted')
ALTER TABLE dbo.LabPanels ADD CONSTRAINT DF_LabPanels_IsDeleted DEFAULT(0) FOR IsDeleted;

IF COL_LENGTH('dbo.LabPanels','CreatedAt') IS NOT NULL
AND NOT EXISTS (SELECT 1 FROM sys.default_constraints dc
  JOIN sys.columns c ON c.default_object_id = dc.object_id
  WHERE dc.parent_object_id = OBJECT_ID('dbo.LabPanels') AND c.name = 'CreatedAt')
ALTER TABLE dbo.LabPanels ADD CONSTRAINT DF_LabPanels_CreatedAt DEFAULT (SYSUTCDATETIME()) FOR CreatedAt;

-- LabPanelItems
IF COL_LENGTH('dbo.LabPanelItems','IsDeleted') IS NOT NULL
AND NOT EXISTS (SELECT 1 FROM sys.default_constraints dc
  JOIN sys.columns c ON c.default_object_id = dc.object_id
  WHERE dc.parent_object_id = OBJECT_ID('dbo.LabPanelItems') AND c.name = 'IsDeleted')
ALTER TABLE dbo.LabPanelItems ADD CONSTRAINT DF_LabPanelItems_IsDeleted DEFAULT(0) FOR IsDeleted;

IF COL_LENGTH('dbo.LabPanelItems','CreatedAt') IS NOT NULL
AND NOT EXISTS (SELECT 1 FROM sys.default_constraints dc
  JOIN sys.columns c ON c.default_object_id = dc.object_id
  WHERE dc.parent_object_id = OBJECT_ID('dbo.LabPanelItems') AND c.name = 'CreatedAt')
ALTER TABLE dbo.LabPanelItems ADD CONSTRAINT DF_LabPanelItems_CreatedAt DEFAULT (SYSUTCDATETIME()) FOR CreatedAt;

-- LabRequests
IF COL_LENGTH('dbo.LabRequests','IsDeleted') IS NOT NULL
AND NOT EXISTS (SELECT 1 FROM sys.default_constraints dc
  JOIN sys.columns c ON c.default_object_id = dc.object_id
  WHERE dc.parent_object_id = OBJECT_ID('dbo.LabRequests') AND c.name = 'IsDeleted')
ALTER TABLE dbo.LabRequests ADD CONSTRAINT DF_LabRequests_IsDeleted DEFAULT(0) FOR IsDeleted;

IF COL_LENGTH('dbo.LabRequests','CreatedAt') IS NOT NULL
AND NOT EXISTS (SELECT 1 FROM sys.default_constraints dc
  JOIN sys.columns c ON c.default_object_id = dc.object_id
  WHERE dc.parent_object_id = OBJECT_ID('dbo.LabRequests') AND c.name = 'CreatedAt')
ALTER TABLE dbo.LabRequests ADD CONSTRAINT DF_LabRequests_CreatedAt DEFAULT (SYSUTCDATETIME()) FOR CreatedAt;

-- LabRequestItems
IF COL_LENGTH('dbo.LabRequestItems','IsDeleted') IS NOT NULL
AND NOT EXISTS (SELECT 1 FROM sys.default_constraints dc
  JOIN sys.columns c ON c.default_object_id = dc.object_id
  WHERE dc.parent_object_id = OBJECT_ID('dbo.LabRequestItems') AND c.name = 'IsDeleted')
ALTER TABLE dbo.LabRequestItems ADD CONSTRAINT DF_LabRequestItems_IsDeleted DEFAULT(0) FOR IsDeleted;

IF COL_LENGTH('dbo.LabRequestItems','CreatedAt') IS NOT NULL
AND NOT EXISTS (SELECT 1 FROM sys.default_constraints dc
  JOIN sys.columns c ON c.default_object_id = dc.object_id
  WHERE dc.parent_object_id = OBJECT_ID('dbo.LabRequestItems') AND c.name = 'CreatedAt')
ALTER TABLE dbo.LabRequestItems ADD CONSTRAINT DF_LabRequestItems_CreatedAt DEFAULT (SYSUTCDATETIME()) FOR CreatedAt;

-- LabSamples
IF COL_LENGTH('dbo.LabSamples','IsDeleted') IS NOT NULL
AND NOT EXISTS (SELECT 1 FROM sys.default_constraints dc
  JOIN sys.columns c ON c.default_object_id = dc.object_id
  WHERE dc.parent_object_id = OBJECT_ID('dbo.LabSamples') AND c.name = 'IsDeleted')
ALTER TABLE dbo.LabSamples ADD CONSTRAINT DF_LabSamples_IsDeleted DEFAULT(0) FOR IsDeleted;

IF COL_LENGTH('dbo.LabSamples','CreatedAt') IS NOT NULL
AND NOT EXISTS (SELECT 1 FROM sys.default_constraints dc
  JOIN sys.columns c ON c.default_object_id = dc.object_id
  WHERE dc.parent_object_id = OBJECT_ID('dbo.LabSamples') AND c.name = 'CreatedAt')
ALTER TABLE dbo.LabSamples ADD CONSTRAINT DF_LabSamples_CreatedAt DEFAULT (SYSUTCDATETIME()) FOR CreatedAt;

-- LabResults (you already set default for CreatedAt in model, but add if missing)
IF COL_LENGTH('dbo.LabResults','IsDeleted') IS NOT NULL
AND NOT EXISTS (SELECT 1 FROM sys.default_constraints dc
  JOIN sys.columns c ON c.default_object_id = dc.object_id
  WHERE dc.parent_object_id = OBJECT_ID('dbo.LabResults') AND c.name = 'IsDeleted')
ALTER TABLE dbo.LabResults ADD CONSTRAINT DF_LabResults_IsDeleted DEFAULT(0) FOR IsDeleted;

IF COL_LENGTH('dbo.LabResults','CreatedAt') IS NOT NULL
AND NOT EXISTS (SELECT 1 FROM sys.default_constraints dc
  JOIN sys.columns c ON c.default_object_id = dc.object_id
  WHERE dc.parent_object_id = OBJECT_ID('dbo.LabResults') AND c.name = 'CreatedAt')
ALTER TABLE dbo.LabResults ADD CONSTRAINT DF_LabResults_CreatedAt DEFAULT (SYSUTCDATETIME()) FOR CreatedAt;
GO
