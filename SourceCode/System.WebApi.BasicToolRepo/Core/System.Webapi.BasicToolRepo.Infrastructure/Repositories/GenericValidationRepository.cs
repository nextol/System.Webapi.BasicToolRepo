using System;
using System.Linq;
using System.Net;
using Microsoft.Practices.EnterpriseLibrary.Common.Utility;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Webapi.BasicToolRepo.Entities.Validation;
using System.Reflection;
using System.Linq.Expressions;
using System.Webapi.BasicToolRepo.Contracts.InterFaces;

namespace System.Webapi.BasicToolRepo.Infrastructure.Repositories
{
    public class GenericValidationRepository : IIgenericValidationRepository
    {


        #region ValidationRelatedFuntions

        /// <summary>
        /// Uses ModelBindingExceptionHandler (Data Annotations defined on the model)
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="validateAllProperties"></param>
        /// <returns></returns>
        public static ValidationResponse ValidateCustom(object instance, bool validateAllProperties = true)
        {
            RegisterMetadataClass(instance);
            var validationContext = new ValidationContext(instance, null, null);
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(instance, validationContext, validationResults, validateAllProperties);

            return new ValidationResponse()
            {
                IsValid = isValid,
                Results = validationResults
            };
        }
        private static void RegisterMetadataClass(object instance)
        {
            var modelType = instance.GetType();
            var metadataType = GetMetadataType(modelType);

            if (metadataType != null)
            {
                TypeDescriptor.AddProviderTransparent(new AssociatedMetadataTypeTypeDescriptionProvider(modelType, metadataType), modelType);
            }
        }

        /// <summary>
        /// Returns the metadata class type defined by the MetadataTypeAttribute, or null if none is present.
        /// </summary>
        private static Type? GetMetadataType(Type type)
        {
            return type.GetCustomAttribute<MetadataTypeAttribute>(true)?.MetadataClassType;
        }
        #endregion

        // Primary validation method
        public ValidationResponse IsValidRequest<TModel>(TModel instance, bool validateAllProperties = true)
            where TModel : class
        {
            var validationResponse = CreateInitialResponse();

            if (instance == null)
            {
                return CreateInvalidResponse(
                    "Invalid request. Please check the request schema.",
                    HttpStatusCode.OK.ToString()
                );
            }

            ValidateInstance(instance, validationResponse, validateAllProperties);

            if (!validationResponse.IsValid)
            {
                validationResponse.ResponsePreamble.Status = "FAILED";
                validationResponse.ResponsePreamble.StatusCode = ((int)HttpStatusCode.UnprocessableEntity).ToString();
                validationResponse.ResponsePreamble.StatusReason = "Request validation failed";
            }

            return validationResponse;
        }

        // Internal instance validator
        private static void ValidateInstance<TModel>(TModel instance, ValidationResponse response, bool validateAllProperties)
            where TModel : class
        {
            ArgumentNullException.ThrowIfNull(instance);

            var topLevelValidation = ValidateCustom(instance, validateAllProperties);
            AppendValidationResults(topLevelValidation, response);

            foreach (var property in instance.GetType().GetProperties().Where(p => p.PropertyType.IsGenericType))
            {
                var propertyValue = property.GetValue(instance);
                if (propertyValue == null)
                {
                    AddError(response, property.Name, $"{property.Name} is required");
                    response.IsValid = false;
                    continue;
                }

                var childValidation = ValidateCustom(propertyValue, validateAllProperties);
                AppendValidationResults(childValidation, response, property.Name);
            }
        }

        // Append validation errors to response
        private static void AppendValidationResults(ValidationResponse validationResult, ValidationResponse response, string fallbackFieldName = "UnknownField")
        {
            if (validationResult.IsValid || validationResult.Results == null) return;

            response.IsValid = false;

            foreach (var result in validationResult.Results)
            {
                var memberName = result.MemberNames.FirstOrDefault() ?? fallbackFieldName;
                var errorMessage = result.ErrorMessage ?? "Unknown validation error";

                response.ResponsePreamble.Errors.Add(new ErrorInfo
                {
                    FieldName = memberName,
                    ErrorMessage = errorMessage
                });
            }
        }

        // Add a single error to response
        private static void AddError(ValidationResponse response, string fieldName, string message)
        {
            response.ResponsePreamble.Errors.Add(new ErrorInfo
            {
                FieldName = fieldName,
                ErrorMessage = message
            });
        }

        // Factory for a valid response object
        private static ValidationResponse CreateInitialResponse()
        {
            return new ValidationResponse
            {
                IsValid = true,
                ResponsePreamble = new ResponsePreamble
                {
                    Errors = new List<ErrorInfo>()
                }
            };
        }

        // Factory for invalid response
        private static ValidationResponse CreateInvalidResponse(string message, string statusCode)
        {
            return new ValidationResponse
            {
                IsValid = false,
                ResponsePreamble = new ResponsePreamble
                {
                    Status = "FAILED",
                    StatusReason = message,
                    StatusCode = statusCode,
                    Errors = new List<ErrorInfo>()
                }
            };
        }


        #region IDisposable Patteren 
        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // Dispose managed state (managed objects) if needed
                    // No managed resources to dispose in this class as of now
                    // If any IDisposable fields are added in the future, dispose them here
                }

                // free unmanaged resources (unmanaged objects) and override finalizer
                // set large fields to null
                disposedValue = true;
            }
        }

        // override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~GenericValidationRepository() // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
