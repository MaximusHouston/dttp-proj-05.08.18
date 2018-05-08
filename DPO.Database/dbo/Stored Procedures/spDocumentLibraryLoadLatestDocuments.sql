




-- =============================================
-- Author:		Charles Teel
-- Create date: 2015.09.17
-- Description:	Loads the latest documents for all users and partners into the proper library directory
-- =============================================
CREATE PROCEDURE [dbo].[spDocumentLibraryLoadLatestDocuments] 
	-- Add the parameters for the stored procedure here
	  @DaysToLoad		int		= 30
AS
BEGIN
	SET NOCOUNT ON

	DECLARE @RC int,
		@OutLibraryHomeId int,
		@OutLatestDocumentsId int,
		@OutVRVLCPartnerLatestDocumentsId int,
		@JobName			nvarchar(80) = 'SQLLoadLatestDocuments',
		@CurrentRunDate		datetime = GETDATE(),
		@LastRunDate		datetime,
		@DocumentsNewerThan datetime = DATEADD(d, -1 * @DaysToLoad, GETDATE())

	EXECUTE @RC = [dbo].[spDocumentLibraryIdentifyLatestFolders] 
	  @OutLibraryHomeId = @OutLibraryHomeId OUTPUT
	  ,@OutLatestDocumentsId = @OutLatestDocumentsId OUTPUT
	  ,@OutVRVLCPartnerLatestDocumentsId = @OutVRVLCPartnerLatestDocumentsId OUTPUT

	-- Load Run Date
	SELECT
		@LastRunDate = LastRunDate
	FROM
		dbo.JobRuns
	WHERE
		JobName = @JobName

	IF (@LastRunDate IS NULL)
	BEGIN
		SET @LastRunDate = '01/01/1900'

		INSERT INTO dbo.JobRuns
		(
			JobName,
			LastRunDate
		)
		VALUES
		(
			@JobName,
			@CurrentRunDate
		)
	END
	ELSE
	BEGIN
		UPDATE
			dbo.JobRuns
		SET
			LastRunDate = @CurrentRunDate
		WHERE
			JobName = @JobName
	END

	IF (@LastRunDate > @DocumentsNewerThan)
	BEGIN
		SET @DocumentsNewerThan = @LastRunDate
	END

	IF (OBJECT_ID('tempdb..#AllDirectories') IS NOT NULL)
	BEGIN
		DROP TABLE #AllDirectories
	END;

	-- Break out all directories
	WITH DirectoriesCTE AS (
			SELECT
				p.LibraryDirectoryId as BaseLibraryDirectoryId,
				p.ParentId as BaseParentId,
				p.Name as BaseName,
				p.Protected as BaseProtected,
				p.LibraryDirectoryId as LibraryDirectoryId,
				CAST(NULL as int) as ParentId,
				CAST(NULL as varchar(MAX)) as Name,
				CAST(0 as bit) as Protected,
				0 as Steps
			FROM
				dbo.LibraryDirectories p 
		UNION ALL
			SELECT
				d.BaseLibraryDirectoryId as BaseLibraryDirectoryId,
				d.BaseParentId as BaseParentId,
				d.BaseName as BaseName,
				d.BaseProtected as BaseProtected,
				p.LibraryDirectoryId,
				p.ParentId as ParentId,
				p.Name,
				p.Protected,
				d.Steps + 1 as Steps
			FROM
				DirectoriesCTE d
				INNER JOIN dbo.LibraryDirectories p
					ON d.LibraryDirectoryId = p.ParentId
	)

	-- Identify all protected directories
	SELECT 
		LibraryDirectoryId,
		Name,
		CAST(MAX(CAST(BaseProtected as int)) as bit) as Protected
	INTO
		#AllDirectories
	FROM 
		DirectoriesCTE
	WHERE
		LibraryDirectoryId NOT IN (@OutLatestDocumentsId, @OutLibraryHomeId)
	GROUP BY
		LibraryDirectoryId,
		Name
	OPTION 
		(MAXRECURSION 0)

	-- Load Protected Documents
	INSERT INTO dbo.LibraryDocumentRelationships
		(LibraryDirectoryId, LibraryDocumentId)
			SELECT 
				LibraryDirectoryId, LibraryDocumentId
			FROM (
				SELECT DISTINCT
					@OutVRVLCPartnerLatestDocumentsId as LibraryDirectoryId,
					ld.LibraryDocumentId,
					ld.Timestamp
				FROM
					dbo.LibraryDocuments ld
					INNER JOIN dbo.LibraryDocumentRelationships ldr
						ON ld.LibraryDocumentId = ldr.LibraryDocumentId
					INNER JOIN #AllDirectories ad
						ON ldr.LibraryDirectoryId = ad.LibraryDirectoryId
				WHERE
					ad.Protected = 1
					AND Timestamp >= @DocumentsNewerThan
					AND NOT EXISTS (
						SELECT 
							* 
						FROM 
							dbo.LibraryDocumentRelationships 
						WHERE
							LibraryDirectoryId = @OutVRVLCPartnerLatestDocumentsId
							AND LibraryDocumentId = ldr.LibraryDocumentId
					)
				
			) tbl
			ORDER BY 
				[Timestamp] DESC
	-- Load unprotected documents
	INSERT INTO dbo.LibraryDocumentRelationships
		(LibraryDirectoryId, LibraryDocumentId)
			SELECT 
				LibraryDirectoryId, LibraryDocumentId
			FROM (
				SELECT DISTINCT
					@OutLatestDocumentsId as LibraryDirectoryId,
					ld.LibraryDocumentId,
					ld.Timestamp
				FROM
					dbo.LibraryDocuments ld
					INNER JOIN dbo.LibraryDocumentRelationships ldr
						ON ld.LibraryDocumentId = ldr.LibraryDocumentId
					INNER JOIN #AllDirectories ad
						ON ldr.LibraryDirectoryId = ad.LibraryDirectoryId
				WHERE
					ad.Protected = 0
					AND [Timestamp] >= @DocumentsNewerThan
					AND NOT EXISTS (
						SELECT 
							* 
						FROM 
							dbo.LibraryDocumentRelationships 
						WHERE
							LibraryDirectoryId = @OutLatestDocumentsId
							AND LibraryDocumentId = ldr.LibraryDocumentId
					)
			) tbl
			ORDER BY 
				[Timestamp] DESC
END