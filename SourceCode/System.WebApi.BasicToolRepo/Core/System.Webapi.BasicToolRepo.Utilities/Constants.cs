using System.Diagnostics.CodeAnalysis;

namespace System.Webapi.BasicToolRepo.Utilities
{
    /// <summary>
    /// Added for supporting application constants.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class Constants
    {
        #region ErrorMessages
        public const string InvalidCompanyCodeMessage = "Country code & Company code provided is not valid!";
        public const string ConnectionStringNotConfiguredMessage = "Connection string is not configured.";
        public const string DatabaseNameNotSetMessage = "DatabaseName is not set in the request context.";
        public const string UnexpectedErrorMessage = "An unexpected error occurred. Please try again later.";
        public const string UnhandledException = "Unhandled exception";
        public const string FailureMessageNotGet = "No failure details provided.";
        public const string UnknownError = "Unknown error";
        #endregion

        #region LogMessages
        public const string TransactionId = "TransactionId";
        public const string StartExport = "Starting export and upload process.";
        public const string QuerySuccess = "Database query successful.";
        public const string QueryFail = "Database query failed.";
        public const string WriteFile = "Writing DataTable to tilde-delimited file: {FilePath}";
        public const string WriteFileSuccess = "File written successfully: {FilePath}";
        public const string ZipFile = "Zipping file: {FilePath} to {ZipFilePath}";
        public const string ZipFileSuccess = "File zipped successfully: {ZipFilePath}";
        public const string SftpUpload = "Uploading zip file to SFTP: {RemotePath}";
        public const string SftpUploadSuccess = "File uploaded to SFTP successfully: {RemotePath}";
        public const string SftpUploadFail = "SFTP upload failed.";
        public const string ExportUploadFail = "Export and upload failed.";
        public const string ExportUploadSuccess = "File exported and uploaded successfully.";
        public const string CleanupFile = "Cleaning up temporary file: {FilePath}";
        public const string NoFilesFoundMsg = "No files found for today.";
        public const string FilesProcessedMsg = "Files processed successfully.";
        public const string FailedToProcessMsg = "Failed to process files.";
        #endregion

        #region HeaderNames
        public const string CountryCode = "IM-CountryCode";
        public const string CompanyCode = "IM-CompanyCode";
        public const string User = "IM-Username";
        public const string Correlationid = "IM-CorrelationId";
        public const string ImUseremail = "im-useremail";
        public const string ImUseremailid = "im-useremailid";
        public const string ContentType = "contenttype";
        public const string ImSenderId = "im-senderid";
        public const string ImVendorId = "im-vendorid";
        public const string ContentTypeValue = "application/json";
        #endregion

        #region FileConstants
        public const string FileNamePattern = "Export_{0:yyyyMMddHHmmss}.txt";
        public const string ZipExtension = ".zip";
        #endregion
    }
}
