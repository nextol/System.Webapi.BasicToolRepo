using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace System.Webapi.BasicToolRepo.Entities.Models
{
    [ExcludeFromCodeCoverage]
    public class Appsettings
    {
        public GceSystemApiDetails GceSystemApiDetails { get; set; }= new GceSystemApiDetails();
        public X4VApiDetails X4VApiDetails { get; set; } = new X4VApiDetails();
        public required IEnumerable<CompanyCodeConfigs> CompanyCodeConfigs { get; set; }
        public bool IsSystemApiEnable {  get; set; }=false;
    }
    [ExcludeFromCodeCoverage]
    public class GceSystemApiDetails:ApiCredentials
    {

    }
    [ExcludeFromCodeCoverage]
    public class X4VApiDetails : ApiCredentials
    {
        public string Senderid { get; set; } = string.Empty;
        public string Source { get; set; } = string.Empty;
        public string CountryCode { get; set; } = string.Empty;
        public string Vendorid { get; set; } = string.Empty;
        public string Correlationid { get; set; } = string.Empty;
    }
    [ExcludeFromCodeCoverage]
    public class DbSchemaConfig
    {
        public string CountryCode { get; set; } = string.Empty;
        public string CompanyCode { get; set; } = string.Empty;
        public string GceSchema { get; set; } = string.Empty;
    }
    [ExcludeFromCodeCoverage]
    public class CompanyCodeConfigs
    {
        public required string CountryCode { get; set; }

        public required string CompanyCode { get; set; }

        public required string SkCompanyCode { get; set; }

        public required string DatabaseName { get; set; }

        public required string DatabaseNameHistory { get; set; }

        public required string CountryCodeThreeChar { get; set; }
    }
    [ExcludeFromCodeCoverage]
    public abstract class ApiCredentials
    {
        public string URL { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

    }
}
