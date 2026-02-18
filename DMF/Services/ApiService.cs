using System.Net.Http.Json;

namespace DMF
{
    public class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;

        public ApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // -------------------- GET --------------------
        public async Task<ApiResponse<T>> GetAsync<T>(string endpoint)
        {
            try
            {
                var response = await _httpClient.GetAsync(endpoint);

                if (!response.IsSuccessStatusCode)
                {
                    return new ApiResponse<T>
                    {
                        Success = false,
                        Message = "API call failed"
                    };
                }

                var result = await response.Content
                                           .ReadFromJsonAsync<ApiResponse<T>>();

                return result ?? new ApiResponse<T>
                {
                    Success = false,
                    Message = "Empty response"
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        // -------------------- POST --------------------
        public async Task<ApiResponse<TResponse>> PostAsync<TRequest, TResponse>(string endpoint, TRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(endpoint, request);

                if (!response.IsSuccessStatusCode)
                {
                    return new ApiResponse<TResponse>
                    {
                        Success = false,
                        Message = "API call failed"
                    };
                }

                var result = await response.Content
                                           .ReadFromJsonAsync<ApiResponse<TResponse>>();

                return result ?? new ApiResponse<TResponse>
                {
                    Success = false,
                    Message = "Empty response"
                };
            }
            catch (Exception ex)
            {
                var message = ex.Message;
                Console.WriteLine("Exception Message: " + ex.ToString());
                Console.WriteLine("Inner Exception: " + ex.InnerException?.Message);
                throw;
            }
        }

        // -------------------- PUT --------------------
        public async Task<ApiResponse<TResponse>> PutAsync<TRequest, TResponse>(
            string endpoint, TRequest request)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync(endpoint, request);

                if (!response.IsSuccessStatusCode)
                {
                    return new ApiResponse<TResponse>
                    {
                        Success = false,
                        Message = "API call failed"
                    };
                }

                var result = await response.Content
                                           .ReadFromJsonAsync<ApiResponse<TResponse>>();

                return result ?? new ApiResponse<TResponse>
                {
                    Success = false,
                    Message = "Empty response"
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        // -------------------- DELETE --------------------
        public async Task<ApiResponse<T>> DeleteAsync<T>(string endpoint)
        {
            try
            {
                var response = await _httpClient.DeleteAsync(endpoint);

                if (!response.IsSuccessStatusCode)
                {
                    return new ApiResponse<T>
                    {
                        Success = false,
                        Message = "API call failed"
                    };
                }

                var result = await response.Content
                                           .ReadFromJsonAsync<ApiResponse<T>>();

                return result ?? new ApiResponse<T>
                {
                    Success = false,
                    Message = "Empty response"
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
