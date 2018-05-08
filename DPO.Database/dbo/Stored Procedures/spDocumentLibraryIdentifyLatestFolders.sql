


-- =============================================
-- Author:		Charles Teel
-- Create date: 2015.09.16
-- Description:	Build the latest documents library folders
-- =============================================
CREATE PROCEDURE [dbo].[spDocumentLibraryIdentifyLatestFolders] (
	@LibraryHomeName					nvarchar(MAX) = 'Library Home',
	@LatestDocumentsName				nvarchar(MAX) = 'Recently Uploaded Documents',
	@VRVLCPartnerLatestDocumentsName	nvarchar(MAX) = 'VRV/LC Sales Partners',
	@OutLibraryHomeId					int OUTPUT,
	@OutLatestDocumentsId				int OUTPUT,
	@OutVRVLCPartnerLatestDocumentsId	int OUTPUT
)
AS
BEGIN
	SET NOCOUNT ON;

	SELECT
		@OutLibraryHomeId = LibraryDirectoryId
	FROM
		dbo.LibraryDirectories
	WHERE 
		Name = @LibraryHomeName

	SELECT TOP 1 
		@OutLatestDocumentsId = LibraryDirectoryId
	FROM 
		dbo.LibraryDirectories
	WHERE 
		Name = @LatestDocumentsName

	IF (@OutLatestDocumentsId IS NULL)
	BEGIN
		BEGIN TRAN t1

		BEGIN TRY
			INSERT INTO dbo.LibraryDirectories
			(
				ParentId,
				Name,
				Protected
			)
			VALUES
			(
				@OutLibraryHomeId,
				@LatestDocumentsName,
				0
			)

			SELECT
				@OutLatestDocumentsId = SCOPE_IDENTITY()

		END TRY
		BEGIN CATCH

			IF (@@TRANCOUNT > 0)
				ROLLBACK TRAN t1

			PRINT ERROR_MESSAGE()
		END CATCH

		COMMIT TRAN t1
	END


	SELECT
		@OutVRVLCPartnerLatestDocumentsId = LibraryDirectoryId
	FROM
		dbo.LibraryDirectories
	WHERE
		Name = @VRVLCPartnerLatestDocumentsName

	IF (@OutVRVLCPartnerLatestDocumentsId IS NULL)
	BEGIN
		BEGIN TRAN t2

		BEGIN TRY
			INSERT INTO dbo.LibraryDirectories
			(
				ParentId,
				Name,
				Protected
			)
			VALUES
			(
				@OutLatestDocumentsId,
				@VRVLCPartnerLatestDocumentsName,
				1
			)

			SELECT
				@OutVRVLCPartnerLatestDocumentsId = SCOPE_IDENTITY()

		END TRY
		BEGIN CATCH

			IF (@@TRANCOUNT > 0)
				ROLLBACK TRAN t2

			PRINT ERROR_MESSAGE()
		END CATCH

		COMMIT TRAN t2

	END
END