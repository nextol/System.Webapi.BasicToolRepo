using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;
using System.Webapi.BasicToolRepo.Contracts.InterFaces;

namespace System.Webapi.BasicToolRepo.Logging
{
    [ExcludeFromCodeCoverage]
    public class Logger<T>: IBasicToolLogger<T> where T : class
    {
        private readonly Microsoft.Extensions.Logging.ILogger<T> _logger;
        public Logger(Microsoft.Extensions.Logging.ILogger<T> logger)
        {
            _logger = logger;
        }
        public void Debug(string message)
        {
            _logger.LogDebug(Sanitizer.SanitizeInput(message));
        }
        public void Debug(string message, string objToLog)
        {
            _logger.LogDebug(Sanitizer.SanitizeInput(message), Sanitizer.SanitizeInput(objToLog));
        }
        public void Debug(Exception exception, string message)
        {
            _logger.LogDebug(exception, Sanitizer.SanitizeInput(message));
        }
        public void Info(string message)
        {
            _logger.LogInformation(Sanitizer.SanitizeInput(message));
        }
        public void Info(Exception exception, string message)
        {
            _logger.LogInformation(exception, Sanitizer.SanitizeInput(message));
        }
        // <summary>
        /// Logs an informational message with template parameters.
        /// </summary>
        /// <param name="message">The message template.</param>
        /// <param name="templateParams">Parameters to format into the message template.</param>
        /// <remarks>
        /// Use case: Log structured information with dynamic values.
        /// </remarks>
        public void Info(string message, params object[] templateParams)
        {
            _logger.LogInformation(message, templateParams);
        }

        /// <summary>
        /// Logs an informational message with an exception and template parameters.
        /// </summary>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">The message template.</param>
        /// <param name="templateParams">Parameters to format into the message template.</param>
        /// <remarks>
        /// Use case: Log structured information and exception details with dynamic values.
        /// </remarks>
        public void Info(Exception exception, string message, params object[] templateParams)
        {
            _logger.LogInformation(exception, message, templateParams);
        }
        /// <summary>
        /// Logs a warning message with an exception.
        /// </summary>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">The warning message to log.</param>
        /// <remarks>
        /// Use case: Log warnings that include exception details.
        /// </remarks>
        public void Warn(Exception exception, string message)
        {
            _logger.LogInformation(exception, Sanitizer.SanitizeInput(message));
        }
        /// <summary>
        /// Logs an error message.
        /// </summary>
        /// <param name="message">The error message to log.</param>
        /// <remarks>
        /// Use case: Log errors that do not require exception details.
        /// </remarks>
        public void Error(string message)
        {
            _logger.LogError(Sanitizer.SanitizeInput(message));
        }

        /// <summary>
        /// Logs an error message with an associated identifier.
        /// </summary>
        /// <param name="message">The error message to log.</param>
        /// <param name="Id">An identifier for the error event.</param>
        /// <remarks>
        /// Use case: Log errors with a specific error code or identifier for tracking.
        /// </remarks>
        public void Error(string message, int Id)
        {
            _logger.LogError(Sanitizer.SanitizeInput(message), Id);
        }

        /// <summary>
        /// Logs an error message with an exception.
        /// </summary>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">The error message to log.</param>
        /// <remarks>
        /// Use case: Log errors that include exception details for diagnostics.
        /// </remarks>
        public void Error(Exception exception, string message)
        {
            _logger.LogError(exception, Sanitizer.SanitizeInput(message));
        }
        /// <summary>
        /// Logs the execution time of an asynchronous operation, including start, completion, and failure events.
        /// </summary>
        /// <typeparam name="T">The return type of the operation.</typeparam>
        /// <param name="operation">The asynchronous operation to execute and measure.</param>
        /// <param name="operationName">A descriptive name for the operation.</param>
        /// <param name="transactionId">A transaction or correlation identifier.</param>
        /// <param name="extraInfo">Optional extra information to include in the log.</param>
        /// <returns>The result of the asynchronous operation.</returns>
        /// <remarks>
        /// Use case: Measure and log the duration of critical or performance-sensitive operations.
        /// </remarks>
        public async Task<Tmodel> LogExecutionTimeAsync<Tmodel>(Func<Task<Tmodel>> operation, string operationName, string transactionId, string? extraInfo = null)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                Info($"{operationName} started. Transaction ID: {transactionId}{(extraInfo != null ? $". {extraInfo}" : "")}");
                var result = await operation();
                stopwatch.Stop();
                Info($"{operationName} completed. Transaction ID: {transactionId}. Duration: {stopwatch.ElapsedMilliseconds} ms{(extraInfo != null ? $". {extraInfo}" : "")}");
                return result;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                Error(ex, $"{operationName} failed. Transaction ID: {transactionId}. Duration: {stopwatch.ElapsedMilliseconds} ms{(extraInfo != null ? $". {extraInfo}" : "")}");
                throw;
            }
        }
    }
}
