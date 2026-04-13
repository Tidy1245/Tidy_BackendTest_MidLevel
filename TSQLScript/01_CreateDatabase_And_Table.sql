-- =============================================
-- Script: 01_CreateDatabase_And_Table.sql
-- Description: 建立 Myoffice_ACPD 資料庫與資料表
-- Date: 2026-04-13
-- =============================================

-- 建立資料庫（若不存在）
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'Myoffice_ACPD')
BEGIN
    CREATE DATABASE [Myoffice_ACPD];
    PRINT '資料庫 Myoffice_ACPD 已建立';
END
GO

USE [Myoffice_ACPD];
GO

-- 建立 MyOffice_ACPD 資料表（若不存在）
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'MyOffice_ACPD' AND type = 'U')
BEGIN

    CREATE TABLE [dbo].[MyOffice_ACPD](
        [ACPD_SID]          [char](20)      NOT NULL,
        [ACPD_Cname]        [nvarchar](60)  NULL,
        [ACPD_Ename]        [nvarchar](40)  NULL,
        [ACPD_Sname]        [nvarchar](40)  NULL,
        [ACPD_Email]        [nvarchar](60)  NULL,
        [ACPD_Status]       [tinyint]       NULL,
        [ACPD_Stop]         [bit]           NULL,
        [ACPD_StopMemo]     [nvarchar](60)  NULL,
        [ACPD_LoginID]      [nvarchar](30)  NULL,
        [ACPD_LoginPWD]     [nvarchar](60)  NULL,
        [ACPD_Memo]         [nvarchar](600) NULL,
        [ACPD_NowDateTime]  [datetime]      NULL,
        [ACPD_NowID]        [nvarchar](20)  NULL,
        [ACPD_UPDDateTime]  [datetime]      NULL,
        [ACPD_UPDID]        [nvarchar](20)  NULL,
        CONSTRAINT [PK_MyOffice_ACPD] PRIMARY KEY CLUSTERED
        (
            [ACPD_SID] ASC
        )
    );

    ALTER TABLE [dbo].[MyOffice_ACPD]
        ADD CONSTRAINT [DF_MyOffice_ACPD_acpd_status]
        DEFAULT ((0)) FOR [ACPD_Status];

    ALTER TABLE [dbo].[MyOffice_ACPD]
        ADD CONSTRAINT [DF_MyOffice_ACPD_acpd_stop]
        DEFAULT ((0)) FOR [ACPD_Stop];

    ALTER TABLE [dbo].[MyOffice_ACPD]
        ADD CONSTRAINT [DF_MyOffice_ACPD_acpd_nowdatetime]
        DEFAULT (getdate()) FOR [ACPD_NowDateTime];

    ALTER TABLE [dbo].[MyOffice_ACPD]
        ADD CONSTRAINT [DF_MyOffice_ACPD_acpd_upddatetime]
        DEFAULT (getdate()) FOR [ACPD_UPDDateTime];

    PRINT '資料表 MyOffice_ACPD 已建立';
END
ELSE
BEGIN
    PRINT '資料表 MyOffice_ACPD 已存在，略過建立';
END
GO
