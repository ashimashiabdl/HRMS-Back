USE [CoreHR]
GO

-- =============================================
-- Performance Testing Script for User_Login_History
-- Purpose: Compare query performance before and after index creation
-- Date: 2025-11-20
-- =============================================

SET NOCOUNT ON;
GO

PRINT '=================================================='
PRINT 'Performance Testing - User Login History Queries'
PRINT '=================================================='
PRINT ''

-- Variables for testing
DECLARE @TestUserId BIGINT;
DECLARE @StartTime DATETIME2;
DECLARE @EndTime DATETIME2;
DECLARE @Duration INT;

-- Get a random user ID from the table for testing
SELECT TOP 1 @TestUserId = AspNetUserId 
FROM [Identity].[User_Login_History]
WHERE AspNetUserId IS NOT NULL
GROUP BY AspNetUserId
ORDER BY COUNT(*) DESC;

IF @TestUserId IS NULL
BEGIN
    PRINT 'ERROR: No test data found in User_Login_History table'
    PRINT 'Please insert some test data before running this script'
    RETURN;
END

PRINT 'Test User ID: ' + CAST(@TestUserId AS NVARCHAR(20))
PRINT ''

-- =============================================
-- TEST 1: Query for Successful Logins
-- =============================================
PRINT '=================================================='
PRINT 'TEST 1: Successful Logins Query'
PRINT '=================================================='

SET STATISTICS IO ON;
SET STATISTICS TIME ON;

SET @StartTime = SYSDATETIME();

SELECT TOP 5
    Id,
    AspNetUserId,
    IsSuccess,
    CreateDate,
    IPAddress,
    IsDeleted
FROM [Identity].[User_Login_History]
WHERE AspNetUserId = @TestUserId 
  AND IsSuccess = 1
  AND IsDeleted = 0
ORDER BY CreateDate DESC;

SET @EndTime = SYSDATETIME();
SET @Duration = DATEDIFF(MILLISECOND, @StartTime, @EndTime);

PRINT ''
PRINT 'Execution Time: ' + CAST(@Duration AS NVARCHAR(10)) + ' ms'
PRINT ''

SET STATISTICS IO OFF;
SET STATISTICS TIME OFF;

-- =============================================
-- TEST 2: Query for Failed Logins
-- =============================================
PRINT '=================================================='
PRINT 'TEST 2: Failed Logins Query'
PRINT '=================================================='

SET STATISTICS IO ON;
SET STATISTICS TIME ON;

SET @StartTime = SYSDATETIME();

SELECT TOP 5
    Id,
    AspNetUserId,
    IsSuccess,
    CreateDate,
    IPAddress,
    IsDeleted
FROM [Identity].[User_Login_History]
WHERE AspNetUserId = @TestUserId 
  AND IsSuccess = 0
  AND IsDeleted = 0
ORDER BY CreateDate DESC;

SET @EndTime = SYSDATETIME();
SET @Duration = DATEDIFF(MILLISECOND, @StartTime, @EndTime);

PRINT ''
PRINT 'Execution Time: ' + CAST(@Duration AS NVARCHAR(10)) + ' ms'
PRINT ''

SET STATISTICS IO OFF;
SET STATISTICS TIME OFF;

-- =============================================
-- TEST 3: Combined Query (Both Success and Fail)
-- =============================================
PRINT '=================================================='
PRINT 'TEST 3: Combined Query (Success + Fail)'
PRINT '=================================================='

SET STATISTICS IO ON;
SET STATISTICS TIME ON;

SET @StartTime = SYSDATETIME();

SELECT TOP 10
    Id,
    AspNetUserId,
    IsSuccess,
    CreateDate,
    IPAddress,
    IsDeleted
FROM [Identity].[User_Login_History]
WHERE AspNetUserId = @TestUserId 
  AND IsDeleted = 0
ORDER BY CreateDate DESC;

SET @EndTime = SYSDATETIME();
SET @Duration = DATEDIFF(MILLISECOND, @StartTime, @EndTime);

PRINT ''
PRINT 'Execution Time: ' + CAST(@Duration AS NVARCHAR(10)) + ' ms'
PRINT ''

SET STATISTICS IO OFF;
SET STATISTICS TIME OFF;

-- =============================================
-- Show Execution Plans Info
-- =============================================
PRINT '=================================================='
PRINT 'Index Usage Statistics'
PRINT '=================================================='

SELECT 
    i.name AS IndexName,
    ISNULL(s.user_seeks, 0) AS UserSeeks,
    ISNULL(s.user_scans, 0) AS UserScans,
    ISNULL(s.user_lookups, 0) AS UserLookups,
    ISNULL(s.user_updates, 0) AS UserUpdates,
    ISNULL(s.last_user_seek, '') AS LastUserSeek,
    ISNULL(s.last_user_scan, '') AS LastUserScan
FROM sys.indexes i
LEFT JOIN sys.dm_db_index_usage_stats s 
    ON i.object_id = s.object_id 
    AND i.index_id = s.index_id
    AND s.database_id = DB_ID()
WHERE i.object_id = OBJECT_ID('Identity.User_Login_History')
  AND i.name IS NOT NULL
ORDER BY (ISNULL(s.user_seeks, 0) + ISNULL(s.user_scans, 0) + ISNULL(s.user_lookups, 0)) DESC;

-- =============================================
-- Table Statistics
-- =============================================
PRINT ''
PRINT '=================================================='
PRINT 'Table Statistics'
PRINT '=================================================='

SELECT 
    TotalRecords = COUNT(*),
    SuccessfulLogins = SUM(CASE WHEN IsSuccess = 1 THEN 1 ELSE 0 END),
    FailedLogins = SUM(CASE WHEN IsSuccess = 0 THEN 1 ELSE 0 END),
    UniqueUsers = COUNT(DISTINCT AspNetUserId),
    DeletedRecords = SUM(CASE WHEN IsDeleted = 1 THEN 1 ELSE 0 END)
FROM [Identity].[User_Login_History];

-- =============================================
-- Missing Index Recommendations
-- =============================================
PRINT ''
PRINT '=================================================='
PRINT 'Missing Index Recommendations'
PRINT '=================================================='

SELECT 
    OBJECT_NAME(d.object_id) AS TableName,
    d.equality_columns AS EqualityColumns,
    d.inequality_columns AS InequalityColumns,
    d.included_columns AS IncludedColumns,
    s.avg_user_impact AS AvgUserImpact,
    s.user_seeks AS UserSeeks,
    s.user_scans AS UserScans
FROM sys.dm_db_missing_index_details d
INNER JOIN sys.dm_db_missing_index_groups g ON d.index_handle = g.index_handle
INNER JOIN sys.dm_db_missing_index_group_stats s ON g.index_group_handle = s.group_handle
WHERE OBJECT_NAME(d.object_id) = 'User_Login_History'
  AND d.database_id = DB_ID()
ORDER BY s.avg_user_impact DESC;

-- =============================================
-- Index Fragmentation
-- =============================================
PRINT ''
PRINT '=================================================='
PRINT 'Index Fragmentation Analysis'
PRINT '=================================================='

SELECT 
    OBJECT_NAME(ips.object_id) AS TableName,
    i.name AS IndexName,
    ips.index_type_desc AS IndexType,
    ips.avg_fragmentation_in_percent AS FragmentationPercent,
    ips.page_count AS PageCount,
    CASE 
        WHEN ips.avg_fragmentation_in_percent > 30 THEN 'REBUILD Recommended'
        WHEN ips.avg_fragmentation_in_percent > 10 THEN 'REORGANIZE Recommended'
        ELSE 'OK'
    END AS Recommendation
FROM sys.dm_db_index_physical_stats(DB_ID(), OBJECT_ID('Identity.User_Login_History'), NULL, NULL, 'LIMITED') ips
INNER JOIN sys.indexes i ON ips.object_id = i.object_id AND ips.index_id = i.index_id
WHERE i.name IS NOT NULL
ORDER BY ips.avg_fragmentation_in_percent DESC;

-- =============================================
-- Cache Hit Ratio
-- =============================================
PRINT ''
PRINT '=================================================='
PRINT 'Buffer Cache Analysis'
PRINT '=================================================='

SELECT 
    OBJECT_NAME(p.object_id) AS TableName,
    i.name AS IndexName,
    COUNT(*) / 128 AS BufferSizeMB,
    COUNT(*) AS CachedPages
FROM sys.dm_os_buffer_descriptors AS bd
INNER JOIN sys.allocation_units AS au ON bd.allocation_unit_id = au.allocation_unit_id
INNER JOIN sys.partitions AS p ON au.container_id = p.hobt_id
INNER JOIN sys.indexes AS i ON p.object_id = i.object_id AND p.index_id = i.index_id
WHERE p.object_id = OBJECT_ID('Identity.User_Login_History')
  AND bd.database_id = DB_ID()
GROUP BY p.object_id, i.name
ORDER BY COUNT(*) DESC;

PRINT ''
PRINT '=================================================='
PRINT 'Performance Testing Completed'
PRINT '=================================================='
PRINT ''
PRINT 'Performance Benchmarks:'
PRINT '  - Excellent: < 50ms'
PRINT '  - Good:      50-200ms'
PRINT '  - Fair:      200-500ms'
PRINT '  - Poor:      > 500ms'
PRINT ''
PRINT 'Index Usage:'
PRINT '  - Index Seek:  ✅ Best (uses index efficiently)'
PRINT '  - Index Scan:  ⚠️  Fair (scans entire index)'
PRINT '  - Table Scan:  ❌ Poor (scans entire table)'
PRINT ''
GO

