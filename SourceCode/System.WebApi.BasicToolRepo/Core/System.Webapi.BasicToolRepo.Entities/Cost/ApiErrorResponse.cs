using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Webapi.BasicToolRepo.Entities.Cost
{
    [ExcludeFromCodeCoverage]
    public class ApiErrorResponse
    {
        public string? Message { get; set; }
        public object? Errors { get; set; }
        public string? TransactionId { get; set; }
    }
}
