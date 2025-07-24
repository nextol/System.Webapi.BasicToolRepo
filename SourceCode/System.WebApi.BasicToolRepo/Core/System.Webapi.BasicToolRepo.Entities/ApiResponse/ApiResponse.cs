#nullable enable
using System.Diagnostics.CodeAnalysis;

namespace System.Webapi.BasicToolRepo.Entities.ApiResponse
{
    /// <summary>
    /// Represents a standardized API response wrapper that encapsulates the result of an API operation.
    /// Includes success status, message, data payload, error details, and transaction identifier.
    /// </summary>
    /// <typeparam name="T">The type of the data payload returned by the API.</typeparam>
    /// <remarks>
    ///     <para>
    ///         <b>Use Cases:</b>
    ///         <list type="bullet">
    ///             <item>
    ///                 <description>
    ///                     Use ApiResponse as the return type for all API endpoints to provide a consistent response structure.
    ///                 </description>
    ///             </item>
    ///             <item>
    ///                 <description>
    ///                     Set Success to true for successful operations and false for failures.
    ///                 </description>
    ///             </item>
    ///             <item>
    ///                 <description>
    ///                     Use Data to return the result of the operation (e.g., an entity, a list, or any value).
    ///                 </description>
    ///             </item>
    ///             <item>
    ///                 <description>
    ///                     Use Message to provide a human-readable description of the result.
    ///                 </description>
    ///             </item>
    ///             <item>
    ///                 <description>
    ///                     Use Errors to return validation or business errors when Success is false.
    ///                 </description>
    ///             </item>
    ///             <item>
    ///                 <description>
    ///                     Use TransactionId to correlate requests and responses for tracing and debugging.
    ///                 </description>
    ///             </item>
    ///         </list>
    ///     </para>
    ///     <para>
    ///         <b>Example (Success):</b>
    ///             <code language="json">
    ///             {
    ///               "success": true,
    ///               "message": "Operation completed successfully.",
    ///               "data": { ... },
    ///               "transactionId": "abc123"
    ///             }
    ///             </code>
    ///         <b>Example (Failure):</b>
    ///             <code language="json">
    ///             {
    ///               "success": false,
    ///               "message": "Validation failed.",
    ///               "errors": [ "Name is required." ],
    ///               "transactionId": "abc123"
    ///             }
    ///         </code>
    ///     </para>
    /// </remarks>
    [ExcludeFromCodeCoverage]
    public class ApiResponse<T>
    {
        public T? Data { get; set; }

        public bool Success { get; set; }
        public int StatusCode { get; set; }

        public string? Message { get; set; }

        public List<string>? Errors { get; set; }

        public string? TransactionId { get; set; }

        [ExcludeFromCodeCoverage]
        public static ApiResponse<T> Ok(T data, string message = "", string? transactionId = null) =>
            new ApiResponse<T> { Success = true, Data = data, Message = message, TransactionId = transactionId };

        [ExcludeFromCodeCoverage]
        public static ApiResponse<T> Fail(string message, List<string>? errors = null, string? transactionId = null) =>
            new ApiResponse<T> { Success = false, Message = message, Errors = errors, TransactionId = transactionId };
    }
}
