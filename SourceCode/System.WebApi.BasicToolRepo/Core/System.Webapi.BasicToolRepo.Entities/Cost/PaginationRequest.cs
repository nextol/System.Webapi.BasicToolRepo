using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Webapi.BasicToolRepo.Entities.Cost
{
    [ExcludeFromCodeCoverage]
    public abstract class PaginationRequest
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
