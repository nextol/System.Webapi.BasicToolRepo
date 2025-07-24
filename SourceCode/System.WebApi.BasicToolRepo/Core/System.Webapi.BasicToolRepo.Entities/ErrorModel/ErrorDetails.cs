
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

using Newtonsoft.Json;

namespace System.Webapi.BasicToolRepo.Entities.ErrorModel
{
    [ExcludeFromCodeCoverage]
    public class ErrorDetails
    {
        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public override string ToString() => JsonConvert.SerializeObject(this);
    }
}
