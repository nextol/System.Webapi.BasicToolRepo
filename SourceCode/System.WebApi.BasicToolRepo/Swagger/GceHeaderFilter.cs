using System.Webapi.BasicToolRepo.Utilities;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection.Metadata;

namespace System.WebApi.BasicToolRepo.Swagger
{
    [ExcludeFromCodeCoverage]
    public class GceHeaderFilter : IOperationFilter
    {
        private const string _headerDataType = "string";
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
         {
            var globalAttributes = context.ApiDescription.ActionDescriptor.FilterDescriptors.Select(p => p.Filter);
            var controllerAttributes = context.MethodInfo?.DeclaringType?.GetCustomAttributes(true);
            var methodAttributes = context.MethodInfo?.GetCustomAttributes(true);
            var produceAttributes = globalAttributes
                .Union(controllerAttributes ?? throw new InvalidOperationException())
                .Union(methodAttributes)
                .OfType<SkipAuthHeaderAttribute>()
                .ToList();

            if (produceAttributes.Count != 0)
            {
                return;
            }

            operation.Parameters ??= new List<OpenApiParameter>();
            ////---Temp. additions till latest version of SDK Core is used to add below fields from SwaggerHeaders 
            #region
            // <summary>
            /// Use when we hosted in Global
            /// </summary>
            //operation.Parameters.Add(new OpenApiParameter
            //{
            //    Name = Constants.CountryCode,
            //    In = ParameterLocation.Header,
            //    Schema = new OpenApiSchema { Type = _headerDataType },
            //    Required = true
            //});
            //operation.Parameters.Add(new OpenApiParameter
            //{
            //    Name = Constants.CompanyCode,
            //    In = ParameterLocation.Header,
            //    Schema = new OpenApiSchema { Type = _headerDataType },
            //    Required = true
            //});
            //operation.Parameters.Add(new OpenApiParameter
            //{
            //    Name = Constants.User,
            //    In = ParameterLocation.Header,
            //    Schema = new OpenApiSchema { Type = _headerDataType },
            //    Required = false
            //});
            //operation.Parameters.Add(new OpenApiParameter
            //{
            //    Name = Constants.Correlationid,
            //    In = ParameterLocation.Header,
            //    Schema = new OpenApiSchema { Type = _headerDataType },
            //    Required = false
            //});
            #endregion
            operation.Parameters.Add(new OpenApiParameter
            {
                Name = Constants.ImUseremail,
                In = ParameterLocation.Header,
                Schema = new OpenApiSchema { Type = _headerDataType },
                Required = false
            });
        }
    }
}
