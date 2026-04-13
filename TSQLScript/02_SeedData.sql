-- =============================================
-- Script: 02_SeedData.sql
-- Description: 新增 MyOffice_ACPD 測試資料（10 筆）
-- Date: 2026-04-13
-- =============================================

USE [Myoffice_ACPD];
GO

-- 清除舊測試資料（可選）
-- DELETE FROM [dbo].[MyOffice_ACPD] WHERE ACPD_NowID = 'SEED';

INSERT INTO [dbo].[MyOffice_ACPD]
    (ACPD_SID, ACPD_Cname, ACPD_Ename, ACPD_Sname, ACPD_Email,
     ACPD_Status, ACPD_Stop, ACPD_StopMemo, ACPD_LoginID, ACPD_LoginPWD,
     ACPD_Memo, ACPD_NowDateTime, ACPD_NowID, ACPD_UPDDateTime, ACPD_UPDID)
VALUES
('ZA10301234SEED000001', N'王小明', 'Wang Xiao Ming', 'WangXM',
 'wang@myoffice.com', 0, 0, NULL, 'wangxm01', 'P@ssw0rd01',
 N'測試帳號1', GETDATE(), 'SEED', GETDATE(), 'SEED'),

('ZA10301234SEED000002', N'李大華', 'Li Da Hua', 'LiDH',
 'li@myoffice.com', 0, 0, NULL, 'lidh02', 'P@ssw0rd02',
 N'測試帳號2', GETDATE(), 'SEED', GETDATE(), 'SEED'),

('ZA10301234SEED000003', N'張美玲', 'Zhang Mei Ling', 'ZhangML',
 'zhang@myoffice.com', 0, 0, NULL, 'zhangml03', 'P@ssw0rd03',
 N'測試帳號3', GETDATE(), 'SEED', GETDATE(), 'SEED'),

('ZA10301234SEED000004', N'陳志成', 'Chen Zhi Cheng', 'ChenZC',
 'chen@myoffice.com', 1, 0, NULL, 'chenzc04', 'P@ssw0rd04',
 N'測試帳號4（進階）', GETDATE(), 'SEED', GETDATE(), 'SEED'),

('ZA10301234SEED000005', N'林雅婷', 'Lin Ya Ting', 'LinYT',
 'lin@myoffice.com', 0, 0, NULL, 'linyt05', 'P@ssw0rd05',
 N'測試帳號5', GETDATE(), 'SEED', GETDATE(), 'SEED'),

('ZA10301234SEED000006', N'黃建國', 'Huang Jian Guo', 'HuangJG',
 'huang@myoffice.com', 0, 1, N'帳號暫停使用', 'huangjg06', 'P@ssw0rd06',
 N'測試帳號6（停用）', GETDATE(), 'SEED', GETDATE(), 'SEED'),

('ZA10301234SEED000007', N'吳宗翰', 'Wu Zong Han', 'WuZH',
 'wu@myoffice.com', 0, 0, NULL, 'wuzh07', 'P@ssw0rd07',
 N'測試帳號7', GETDATE(), 'SEED', GETDATE(), 'SEED'),

('ZA10301234SEED000008', N'蔡佳穎', 'Tsai Chia Ying', 'TsaiCY',
 'tsai@myoffice.com', 0, 0, NULL, 'tsaicy08', 'P@ssw0rd08',
 N'測試帳號8', GETDATE(), 'SEED', GETDATE(), 'SEED'),

('ZA10301234SEED000009', N'許文傑', 'Hsu Wen Chieh', 'HsuWC',
 'hsu@myoffice.com', 1, 0, NULL, 'hsuwc09', 'P@ssw0rd09',
 N'測試帳號9（進階）', GETDATE(), 'SEED', GETDATE(), 'SEED'),

('ZA10301234SEED000010', N'鄭淑芬', 'Cheng Shu Fen', 'ChengSF',
 'cheng@myoffice.com', 0, 0, NULL, 'chengsf10', 'P@ssw0rd10',
 N'測試帳號10', GETDATE(), 'SEED', GETDATE(), 'SEED');

PRINT '已新增 10 筆測試資料';
GO
