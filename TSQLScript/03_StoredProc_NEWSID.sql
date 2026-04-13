-- =============================================
-- Script: 03_StoredProc_NEWSID.sql
-- Description: NEWSID 預存程序 — 產生 20 字元唯一識別碼
-- 備註: 本專案使用 C# SidGeneratorService 實作相同邏輯
--       此 SP 保留供參考與資料庫端直接使用
-- =============================================

USE [Myoffice_ACPD];
GO

IF OBJECT_ID('dbo.NEWSID') IS NOT NULL
    DROP PROCEDURE [dbo].[NEWSID]
GO

CREATE PROCEDURE [dbo].[NEWSID]
(
    @TableName  nvarchar(128),
    @ReturnSID  nvarchar(20) OUTPUT
)
AS
BEGIN
    SET NOCOUNT ON

    DECLARE @SIDRowName NVARCHAR(20)

    DECLARE
        @currentYear    int,
        @dayOfYear      int,
        @secondOfDay    int,
        @alphabets      char(36),
        @firstDigit     char(1),
        @secondDigit    char(1),
        @prefix         char(2),
        @dayCode        char(3),
        @secondCode     char(5),
        @sql            nvarchar(MAX),
        @randomValue    char(10),
        @ParmDefinition nvarchar(500)

    DECLARE @tempTable TABLE (SID CHAR(20));

    SET @currentYear = YEAR(GETDATE()) - 2000;
    SET @dayOfYear   = DATEPART(DAYOFYEAR, GETDATE());
    SET @secondOfDay = DATEPART(SECOND, GETDATE())
                     + (60  * DATEPART(MINUTE, GETDATE()))
                     + (3600 * DATEPART(HOUR,   GETDATE()));

    IF (@currentYear > 1295) SET @currentYear = 1295;

    SET @alphabets   = '0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ';
    SET @firstDigit  = SUBSTRING(@alphabets, (@currentYear / 36) % 36 + 1, 1);
    SET @secondDigit = SUBSTRING(@alphabets,  @currentYear % 36      + 1, 1);
    SET @prefix      = @firstDigit + @secondDigit;
    SET @dayCode     = RIGHT('000'   + CONVERT(VARCHAR, @dayOfYear),   3);
    SET @secondCode  = RIGHT('00000' + CONVERT(VARCHAR, @secondOfDay), 5);

    -- 取得主鍵欄位名稱
    SELECT TOP 1
        @SIDRowName = STUFF((
            SELECT ', ' + c.name
            FROM sys.index_columns ic
            JOIN sys.columns c ON ic.object_id = c.object_id AND ic.column_id = c.column_id
            WHERE ic.object_id = i.object_id AND ic.index_id = i.index_id
            ORDER BY ic.key_ordinal
            FOR XML PATH('')
        ), 1, 2, '')
    FROM sys.indexes i
    WHERE i.object_id = OBJECT_ID(@TableName) AND i.index_id > 0;

    -- 唯一性檢查迴圈
    WHILE 1 = 1
    BEGIN
        SET @randomValue = RIGHT(
            '0000000000' + CAST(ABS(CAST(CAST(NEWID() AS BINARY(5)) AS BIGINT)) % 10000000000 AS VARCHAR(10)),
            10
        );
        SET @ReturnSID = @prefix + @dayCode + @secondCode + @randomValue;

        SET @sql = N'SELECT @SIDRowNameOUT = ' + QUOTENAME(@SIDRowName)
                 + N' FROM ' + QUOTENAME(@TableName)
                 + N' WHERE ' + QUOTENAME(@SIDRowName) + N' = @ReturnSIDIN';
        SET @ParmDefinition = N'@ReturnSIDIN nvarchar(20), @SIDRowNameOUT nvarchar(20) OUTPUT';

        DELETE FROM @tempTable;

        DECLARE @SIDRowNameOUT nvarchar(20);
        EXEC sp_executesql @sql, @ParmDefinition,
            @ReturnSIDIN    = @ReturnSID,
            @SIDRowNameOUT  = @SIDRowNameOUT OUTPUT;

        IF @SIDRowNameOUT IS NULL BREAK;
    END
END
GO

PRINT 'NEWSID 預存程序已建立';
GO

-- =============================================
-- 執行日誌資料表（選用）
-- =============================================

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'MyOffice_ExcuteionLog' AND type = 'U')
BEGIN
    CREATE TABLE [dbo].[MyOffice_ExcuteionLog](
        [DeLog_AutoID]          [bigint]          IDENTITY(1,1) NOT NULL,
        [DeLog_StoredPrograms]  [nvarchar](120)   NOT NULL,
        [DeLog_GroupID]         [uniqueidentifier] NOT NULL,
        [DeLog_isCustomDebug]   [bit]             NOT NULL,
        [DeLog_ExecutionProgram][nvarchar](120)   NOT NULL,
        [DeLog_ExecutionInfo]   [nvarchar](max)   NULL,
        [DeLog_verifyNeeded]    [bit]             NULL,
        [DeLog_ExDateTime]      [datetime]        NOT NULL,
        CONSTRAINT [PK_MOTC_DataExchangeLog] PRIMARY KEY CLUSTERED ([DeLog_AutoID] ASC)
    );

    ALTER TABLE [dbo].[MyOffice_ExcuteionLog]
        ADD CONSTRAINT [DF_MOTC_DataExchangeLog_DeLog_isCustomDebug] DEFAULT ((0)) FOR [DeLog_isCustomDebug];
    ALTER TABLE [dbo].[MyOffice_ExcuteionLog]
        ADD CONSTRAINT [DF_MyOffice_ExcuteionLog_DeLog_verifyNeeded] DEFAULT ((0)) FOR [DeLog_verifyNeeded];
    ALTER TABLE [dbo].[MyOffice_ExcuteionLog]
        ADD CONSTRAINT [DF_MOTC_DataExchangeLog_DeLog_ExDateTime]    DEFAULT (getdate()) FOR [DeLog_ExDateTime];

    PRINT '資料表 MyOffice_ExcuteionLog 已建立';
END
GO
