using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Webapi.BasicToolRepo.Entities.RequestContext;
using System.Webapi.BasicToolRepo.Utilities;
using System.Linq;
using System.Webapi.BasicToolRepo.Entities.Models;
using System;
using System.Webapi.BasicToolRepo.Contracts.InterFaces;

namespace System.Webapi.BasicToolRepo.Helper
{
    /// <summary>
    /// Added for supporting request context resolution.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class RequestContextResolver : IRequestContextProvider
    {
        #region Private/Public Fields
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IReadOnlyList<CompanyCodeConfigs> _companyCodeConfigs;
        #endregion

        public RequestContextResolver(IHttpContextAccessor httpContextAccessor, IOptions<Appsettings> appSettings)
        {
            _httpContextAccessor = httpContextAccessor;
            _companyCodeConfigs = appSettings.Value.CompanyCodeConfigs?.ToList() ?? new List<CompanyCodeConfigs>();
        }

        public RequestContext GetCurrentContext()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext is null)
                return null;

            var headers = httpContext.Request.Headers;

            // <summary>
            /// Use when we hosted in Global
            /// </summary>
            var imCorrelationId = headers.TryGetValue(Constants.Correlationid, out var imCorrelationIdValue) && imCorrelationIdValue.Count > 0 ? imCorrelationIdValue[0] : Guid.NewGuid().ToString();
            var imUsername = headers.TryGetValue(Constants.User, out var imUsernameValue) && imUsernameValue.Count > 0 ? imUsernameValue[0] : "APPGCE";
            var countryCode = headers.TryGetValue(Constants.CountryCode, out var countryCodeValues) ? countryCodeValues.ToString() : string.Empty;
            var companyCode = headers.TryGetValue(Constants.CompanyCode, out var companyCodeValues) ? companyCodeValues.ToString() : string.Empty;
            var imUseremail = headers.TryGetValue(Constants.ImUseremail, out var imUseremailValue) && imUseremailValue.Count > 0 ? imUseremailValue[0] : "APPGCE";


            var (skCompanyCode, databaseName, databaseNameHistory, countryCodeThreeChar) = ResolveCompanyCodeConfig(countryCode, companyCode);

            return new RequestContext
            {
                IMCorrelationId = imCorrelationId,
                IMUsername = imUsername,
                ImUseremail = imUseremail,
                CountryCode = countryCode,
                CompanyCode = companyCode,
                SkCompanyCode = skCompanyCode,
                DatabaseName = databaseName,
                DatabaseNameHistory = databaseNameHistory,
                CountryCodeThreeChar = countryCodeThreeChar
            };
        }

        private (long skCompanyCode, string databaseName, string databaseNameHistory,string countryCodeThreeChar) ResolveCompanyCodeConfig(string countryCode, string companyCode)
        {
            if (string.IsNullOrWhiteSpace(countryCode) || string.IsNullOrWhiteSpace(companyCode))
                return (0, string.Empty, string.Empty, string.Empty);

            var companyCodeConfig = _companyCodeConfigs.FirstOrDefault(x =>
                string.Equals(x.CountryCode, countryCode, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(x.CompanyCode, companyCode, StringComparison.OrdinalIgnoreCase));

            if (companyCodeConfig is null)
                throw new InvalidOperationException(Constants.InvalidCompanyCodeMessage);

            if (!long.TryParse(companyCodeConfig.SkCompanyCode, out var skCompanyCode))
                skCompanyCode = 0;

            var databaseName = companyCodeConfig.DatabaseName ?? string.Empty;
            var databaseNameHistory = companyCodeConfig.DatabaseNameHistory ?? string.Empty;
            var countryCodeThreeChar = companyCodeConfig.CountryCodeThreeChar ?? string.Empty;
            return (skCompanyCode, databaseName, databaseNameHistory, countryCodeThreeChar);
        }
    }
}