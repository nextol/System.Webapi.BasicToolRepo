using System.Diagnostics.CodeAnalysis;

namespace System.Webapi.BasicToolRepo.Utilities
{

    /// <summary>
    /// Represents standard HTTP status codes.
    /// </summary>
    public enum HttpStatusCode
    {
        // 2xx Success
        OK = 200,
        Created = 201,
        Accepted = 202,
        NoContent = 204,
        Success,

        // 3xx Redirection
        MovedPermanently = 301,
        Found = 302,
        NotModified = 304,

        // 4xx Client Error
        BadRequest = 400,
        Unauthorized = 401,
        Forbidden = 403,
        NotFound = 404,
        Conflict = 409,
        UnprocessableEntity = 422,

        // 5xx Server Error
        InternalServerError = 500,
        NotImplemented = 501,
        BadGateway = 502,
        ServiceUnavailable = 503,
        GatewayTimeout = 504
    }
}
