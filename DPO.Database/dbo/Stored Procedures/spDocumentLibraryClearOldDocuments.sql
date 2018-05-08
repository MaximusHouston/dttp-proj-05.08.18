

-- =============================================
-- Author:		Charles Teel
-- Create date: 2015.09.17
-- Description:	Loads the latest documents for all users and partners into the proper library directory
-- =============================================
CREATE PROCEDURE [dbo].[spDocumentLibraryClearOldDocuments] 
	-- Add the parameters for the stored procedure here
	  @DaysToClear		int		= 30
AS
BEGIN
	SET NOCOUNT ON

	DECLARE @RC int,
		@OutLibraryHomeId int,
		@OutLatestDocumentsId int,
		@OutVRVLCPartnerLatestDocumentsId int,
		@JobName			nvarchar(80) = 'SQLClearLatestDocuments',
		@CurrentRunDate		datetime = GETDATE(),
		@LastRunDate		datetime,
		@DocumentsNewerThan datetime = DATEADD(d, -1 * @DaysToClear, GETDATE())

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

	DELETE
		dbo.LibraryDocumentRelationships
	FROM
		dbo.LibraryDocuments ld
		INNER JOIN dbo.LibraryDocumentRelationships ldr
			ON ld.LibraryDocumentId = ldr.LibraryDocumentId
	WHERE
		Timestamp <= @DocumentsNewerThan
		AND ldr.LibraryDirectoryId IN (@OutLatestDocumentsId, @OutVRVLCPartnerLatestDocumentsId)
END