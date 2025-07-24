namespace System.Webapi.BasicToolRepo.Entities.RequestContext
{
    /// <summary>
    /// Added for supporting request context operations.
    /// </summary>
    public interface IRequestContextInfo
    {
        string IMCorrelationId { get; set; }

        string IMUsername { get; set; }
        string ImUseremail { get; set; }

        string CountryCode { get; set; }

        string CompanyCode { get; set; }

        long SkCompanyCode { get; set; }

        string DatabaseName { get; set; }

        string DatabaseNameHistory { get; set; }

        bool IsCopCountry { get; set; }
        string CountryCodeThreeChar { get; set; }
    }
}
