USE [CoreHR]
GO

-- =============================================
-- Script: Create Performance Indexes for User_Login_History
-- Description: Optimizes queries for login activity and login fail activity components
-- Date: 2025-11-20
-- =============================================

-- Check if index exists before creating
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_User_Login_History_AspNetUserId_IsSuccess_CreateDate' 
               AND object_id = OBJECT_ID('Identity.User_Login_History'))
BEGIN
    -- Composite index for filtering by user and success status with included CreateDate for sorting
    CREATE NONCLUSTERED INDEX [IX_User_Login_History_AspNetUserId_IsSuccess_CreateDate]
    ON [Identity].[User_Login_History] ([AspNetUserId], [IsSuccess], [CreateDate] DESC)
    INCLUDE ([IPAddress], [IsDeleted])
    WITH (ONLINE = ON, FILLFACTOR = 90);
    
    PRINT 'Index IX_User_Login_History_AspNetUserId_IsSuccess_CreateDate created successfully.';
END
ELSE
BEGIN
    PRINT 'Index IX_User_Login_History_AspNetUserId_IsSuccess_CreateDate already exists.';
END
GO

-- Separate index optimized for successful logins
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_User_Login_History_Success_Logins' 
               AND object_id = OBJECT_ID('Identity.User_Login_History'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_User_Login_History_Success_Logins]
    ON [Identity].[User_Login_History] ([AspNetUserId], [IsSuccess])
    WHERE [IsSuccess] = 1 AND [IsDeleted] = 0
    WITH (ONLINE = ON, FILLFACTOR = 90);
    
    PRINT 'Index IX_User_Login_History_Success_Logins created successfully.';
END
ELSE
BEGIN
    PRINT 'Index IX_User_Login_History_Success_Logins already exists.';
END
GO

-- Separate index optimized for failed logins
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_User_Login_History_Failed_Logins' 
               AND object_id = OBJECT_ID('Identity.User_Login_History'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_User_Login_History_Failed_Logins]
    ON [Identity].[User_Login_History] ([AspNetUserId], [IsSuccess])
    WHERE [IsSuccess] = 0 AND [IsDeleted] = 0
    WITH (ONLINE = ON, FILLFACTOR = 90);
    
    PRINT 'Index IX_User_Login_History_Failed_Logins created successfully.';
END
ELSE
BEGIN
    PRINT 'Index IX_User_Login_History_Failed_Logins already exists.';
END
GO

-- Optional: Update statistics for better query optimization
UPDATE STATISTICS [Identity].[User_Login_History] WITH FULLSCAN;
GO

PRINT 'All indexes created and statistics updated successfully.';
GO

