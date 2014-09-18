USE [Enterprise]
GO
/****** Object:  StoredProcedure [dbo].[SearchProducer]    Script Date: 09/17/2014 16:45:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Jim Schueler
-- Create date: 10.06.2010
-- Description:	Selects a Producer's number, name, city and state based on a keyword
-- Utilized by  the Portal
--
-- Change Log:
-- 11/8/2013 DN - Added Active/Inactive indicators
-- =============================================
ALTER PROCEDURE [dbo].[SearchProducer]
	@searchWord varchar(50),
	@includeActive bit = 1,
	@includeInactive bit = 0
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	--check the producer status
	--0 = inactive lab producer
	--1 = active lab producer
	--2 = active not shipping
	--3 = active, shipping
	--4 = split
	--5 = inactive

	create table #tb ([status_code] tinyint);
	
	IF @includeActive = 1 
		begin
			insert into #tb values (1), (2), (3), (4);
		end
	if @includeInactive = 1
		begin
			insert into #tb values (0), (5);
		end
		
	SELECT Producer_Division, PM.Producer_Number, Farm_Name, City, [State],pm.Producer_Status
	FROM Producer_Master PM JOIN 
		Producer_Contact PC ON PM.Producer_Number = PC.Producer_Number AND PM.Division = PC.Producer_Division LEFT JOIN 
		Contact_Master CM ON PC.Contact_ID = CM.Contact_ID LEFT JOIN 
		Address_Master AM ON AM.Contact_ID = CM.Contact_ID
	WHERE (PM.Producer_Number LIKE '%' + @searchWord + '%'
		OR PM.Division LIKE '%' + @searchWord + '%'
		OR (PM.Division + PM.Producer_Number LIKE '%' + @searchWord + '%')
		OR PM.Farm_Name LIKE '%' + @searchWord + '%'
		OR AM.Address_1 LIKE '%' + @searchWord + '%'
		OR AM.City LIKE '%' + @searchWord + '%'
		OR AM.State LIKE '%' + @searchWord + '%')
		AND Contact_Priority = 1
		AND Address_Type = 2
		AND (pm.Producer_Status IN (SELECT [status_code] from #tb))
		order by Producer_Status , Division, producer_number;
		
	drop table #tb;
END