using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Webapi.BasicToolRepo.Entities.ApiResponse;

namespace System.Webapi.BasicToolRepo.Contracts.InterFaces
{
    public interface IApiClient
    {
        Task<ApiResponse<TResponse>> PostAsync<TRequest, TResponse>(
    string endpoint, string userName, string password, object apiDetails, TRequest request);

        Task<ApiResponse<T>> GetAsync<T>(
    string endpoint, string userName, string password, object apiDetails);

        Task<ApiResponse<TResponse>> PatchAsync<TRequest, TResponse>(
    string endpoint, string userName, string password, object apiDetails, TRequest request);
    }
}
