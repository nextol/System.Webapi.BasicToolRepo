using System.Diagnostics.CodeAnalysis;

namespace System.Webapi.BasicToolRepo.Entities.RequestContext
{
    [ExcludeFromCodeCoverage]
    /// <summary>
    /// Added to support request context information.
    /// </summary>
    public class RequestContext : IRequestContextInfo
    {
        public required string IMCorrelationId { get; set; }

        public required string IMUsername { get; set; }
        public required string ImUseremail { get; set; } 

        public required string CountryCode { get; set; }

        public required string CompanyCode { get; set; }

        public long SkCompanyCode { get; set; }

        public required string DatabaseName { get; set; }

        public required string DatabaseNameHistory { get; set; }

        public bool IsCopCountry { get; set; }

        public required string CountryCodeThreeChar { get; set; }
    }
}
