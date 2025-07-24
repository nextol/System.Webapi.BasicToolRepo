using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Webapi.BasicToolRepo.Entities.Cost
{
    [ExcludeFromCodeCoverage]
    public abstract class BaseServiceResponse
    {
        public string Message { get; set; }=string.Empty;
        public string TransactionStatus { get; set; }=string.Empty;
        public string TransactionId { get; set; } = string.Empty;
    }
}
