/* Drop the tables created by the ProducerCRM mobile app */

--USE [dlBackendAz01]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserIdentities]') AND type in (N'U'))
DROP TABLE [dbo].[UserIdentities]

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[VisitXReasons]') AND type in (N'U'))
DROP TABLE [dbo].[VisitXReasons]

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[StoredProducerVisitReports]') AND type in (N'U'))
DROP TABLE [dbo].[StoredProducerVisitReports]

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ReasonCodes]') AND type in (N'U'))
DROP TABLE [dbo].[ReasonCodes]

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[__MigrationHistory]') AND type in (N'U'))
DROP TABLE [dbo].[__MigrationHistory]
GO