using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace System.Webapi.BasicToolRepo.Entities.Validation
{
    [ExcludeFromCodeCoverage]
    public class ValidationResponse
    {
        //To Do - Create properties for RequestPreamble, BidHeader, BidLines and BidRelations  -- type List<ValidationResult>
        public List<ValidationResult> Results { get; set; }
        public bool IsValid { get; set; }
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public ValidationResponse()
        {
            Results = new List<ValidationResult>();
            IsValid = false;
            StatusCode = 0;
            Message = string.Empty;
            ResponsePreamble=new ResponsePreamble();
        }

        public ResponsePreamble ResponsePreamble { get; set; }

    }
    [ExcludeFromCodeCoverage]
    public class ResponsePreamble
    {

        public string Status { get; set; }=string.Empty;


        public string StatusReason { get; set; } = string.Empty;


        public string StatusCode { get; set; } = string.Empty;
        public List<ErrorInfo> Errors { get; set; }=new List<ErrorInfo>();
    }
    [ExcludeFromCodeCoverage]
    public class ErrorInfo
    {
        public string FieldName { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;
    }
}
