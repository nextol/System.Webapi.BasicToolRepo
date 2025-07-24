
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using System.Webapi.BasicToolRepo.Contracts;
using System.Webapi.BasicToolRepo.Entities.ErrorModel;
using System.Collections.Generic;
using System;
using Microsoft.Extensions.Logging;
using System.Webapi.BasicToolRepo.Contracts.InterFaces;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection.Metadata;
using System.Webapi.BasicToolRepo.Entities.ApiResponse;
using System.Webapi.BasicToolRepo.Utilities;

namespace System.Webapi.BasicToolRepo.Extensions
{
        
    [ExcludeFromCodeCoverage]
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        public ExceptionHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IBasicToolLogger<ExceptionHandlerMiddleware> logger)
        {
            try
            {
                await _next(context);
            }
            catch (Exception error)
            {
                logger.Error(error, Constants.UnhandledException);
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)Utilities.HttpStatusCode.InternalServerError;
                var response = ApiResponse<string>.Fail(Constants.UnexpectedErrorMessage);
                response.Errors = new List<string> { error.Message };
                response.StatusCode = (int)Utilities.HttpStatusCode.InternalServerError;
                await context.Response.WriteAsJsonAsync(response);
            }
        }


        [ExcludeFromCodeCoverage]
        public class ResponsePreamble
        {
            public string Status { get; set; }

            public string StatusReason { get; set; }

            public string StatusCode { get; set; }

            public string AppliedDiscountsRequestedOn { get; set; }

            public List<ErrorInfo> Errors { get; set; }
        }
        [ExcludeFromCodeCoverage]
        public class ErrorInfo
        {
            public string FieldName { get; set; }

            public string ErrorMessage { get; set; }
        }
    }
}
