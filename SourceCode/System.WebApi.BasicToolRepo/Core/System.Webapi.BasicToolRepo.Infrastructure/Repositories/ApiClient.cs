

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Webapi.BasicToolRepo.Entities.Models;
using System.Webapi.BasicToolRepo.Entities.ApiResponse;
using Newtonsoft.Json;
using System.Webapi.BasicToolRepo.Entities.RequestContext;
using System.Webapi.BasicToolRepo.Contracts.InterFaces;
using System.Reflection.Metadata;
using System.Webapi.BasicToolRepo.Utilities;
using System.Webapi.BasicToolRepo.Entities.Cost;
using System.Text.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace System.Webapi.BasicToolRepo.Infrastructure.Repositories
{
    public class ApiClient : IApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly IRequestContextInfo _gceContext;

        public ApiClient(HttpClient httpClient, IRequestContextInfo gceContext)
        {
            _httpClient = httpClient;
            _gceContext = gceContext;
        }

        public async Task<ApiResponse<T>> GetAsync<T>(string endpoint, string userName, string password, object apiDetails)
        {
            // Clear and configure headers
            ConfigureHeaders(userName, password, apiDetails);

            // Call API
            var response = await _httpClient.GetAsync(endpoint);
            return  BuildApiResponse<T>(response);
        }



        public async Task<ApiResponse<TResponse>> PostAsync<TRequest, TResponse>(
            string endpoint, string userName, string password, object apiDetails, TRequest request)
        {
            // Clear and configure headers
            ConfigureHeaders(userName, password, apiDetails);

            // Call API
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(endpoint, content);
            return BuildApiResponse<TResponse>(response);
        }


        public async Task<ApiResponse<TResponse>> PatchAsync<TRequest, TResponse>(
            string endpoint, string userName, string password, object apiDetails, TRequest request)
        {
            // Clear and configure headers
            ConfigureHeaders(userName, password, apiDetails);

            // Call API
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, Constants.ContentTypeValue);
            var response = await _httpClient.PatchAsync(endpoint, content);
            return BuildApiResponse<TResponse>(response);
        }

        #region Helper Methods
        private void ConfigureHeaders(string userName, string password, object apiDetails)
        {
            _httpClient.DefaultRequestHeaders.Clear();
            var header = _httpClient.DefaultRequestHeaders;

            AddHeaderIfNotExists(header, Constants.CountryCode, _gceContext.CountryCode);
            AddHeaderIfNotExists(header, Constants.CompanyCode, _gceContext.CompanyCode);
            AddHeaderIfNotExists(header, Constants.Correlationid, _gceContext.IMCorrelationId);
            AddHeaderIfNotExists(header, Constants.User, _gceContext.IMUsername);
            AddHeaderIfNotExists(header, Constants.ImUseremail, _gceContext.ImUseremail);

            header.Add(Constants.ContentType, Constants.ContentTypeValue);
            if (!header.Accept.Any(h => h.MediaType == Constants.ContentTypeValue))
                header.Accept.Add(new MediaTypeWithQualityHeaderValue(Constants.ContentTypeValue));

            // Handle X4VApiDetails
            if (apiDetails is X4VApiDetails x4v)
            {
                if (header.Contains(Constants.CountryCode))
                    header.Remove(Constants.CountryCode);
                if (header.Contains(Constants.User))
                    header.Remove(Constants.User);

                AddHeaderIfNotExists(header, Constants.CountryCode, _gceContext.CountryCodeThreeChar);
                AddHeaderIfNotExists(header, Constants.ImSenderId, x4v.Senderid);
                AddHeaderIfNotExists(header, Constants.ImVendorId, x4v.Vendorid);
                AddHeaderIfNotExists(header, Constants.ImUseremailid, _gceContext.ImUseremail);

                if (!string.IsNullOrWhiteSpace(x4v.UserName) && !string.IsNullOrWhiteSpace(x4v.Password))
                {
                    var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{x4v.UserName}:{x4v.Password}"));
                    header.Authorization = new AuthenticationHeaderValue("Basic", credentials);
                }
            }
            // Handle GceSystemApiDetails or other types as needed
            else if (apiDetails is GceSystemApiDetails &&
                !string.IsNullOrWhiteSpace(userName) &&
                !string.IsNullOrWhiteSpace(password))
            {
                var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{userName}:{password}"));
                header.Authorization = new AuthenticationHeaderValue("Basic", credentials);
            }

        }

        private static void AddHeaderIfNotExists(HttpRequestHeaders headers, string name, string value)
        {
            if (!headers.Contains(name) && !string.IsNullOrWhiteSpace(value))
            {
                headers.Add(name, value);
            }
        }

        private static ApiResponse<T> BuildApiResponse<T>(dynamic response)
        {
            var apiResponse = new ApiResponse<T>
            {
                StatusCode = (int)response.StatusCode,
                Success = response.IsSuccess,
            };
            string json =  response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
               var jsonData= JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                apiResponse.Data = jsonData is T success ? success : default;
                
            }
            else if (response.FailureResponse is string failureJson && !string.IsNullOrWhiteSpace(failureJson))
            {
                apiResponse.Errors = new List<string>();
                try
                {
                    var errorResponse = JsonConvert.DeserializeObject<ApiErrorResponse>(failureJson);
                    if (errorResponse != null)
                    {
                        if (!string.IsNullOrWhiteSpace(errorResponse.Message))
                            apiResponse.Errors.Add(errorResponse.Message);
                        if (errorResponse.Errors is IEnumerable<string> errorList)
                            apiResponse.Errors.AddRange(errorList.Where(e => !string.IsNullOrWhiteSpace(e)));
                    }
                }
                catch
                {
                    apiResponse.Errors.Add("An unknown error occurred.");
                }
            }

            return apiResponse;
        }
        private async Task<ApiResponse<T>> HandleResponse<T>(HttpResponseMessage response)
        {
            var json = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var data = JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return new ApiResponse<T> { Success = true, Data = data };
            }

            return new ApiResponse<T>
            {
                Success = false,
                Message = $"HTTP {(int)response.StatusCode} - {response.ReasonPhrase}, Body: {json}"
            };
        }
        #endregion
    }
}

