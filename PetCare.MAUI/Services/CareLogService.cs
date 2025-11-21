using System.Net.Http.Json;
using System.Net.Http.Headers;
using PetCare.Shared;
using PetCare.Shared.DTOs;

namespace PetCare.MAUI.Services
{
    // 2. The Implementation
    public class CareLogService : ICareLogService
    {
        private readonly HttpClient _httpClient;
        private readonly UserState _userState;

        public CareLogService(HttpClient httpClient, UserState userState)
        {
            _httpClient = httpClient;
            _userState = userState;
        }

        private void SetAuthorizationHeader()
        {
            if (!string.IsNullOrEmpty(_userState.AuthToken))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", _userState.AuthToken);
            }
        }

        public async Task<List<CareLog>> GetLogsAsync(int petId)
        {
            SetAuthorizationHeader();
            try
            {
                return await _httpClient.GetFromJsonAsync<List<CareLog>>($"api/CareLogs/pet/{petId}")
                       ?? new List<CareLog>();
            }
            catch
            {
                return new List<CareLog>();
            }
        }

        public async Task<bool> AddLogAsync(CreateCareLogDto logDto)
        {
            SetAuthorizationHeader();
            var response = await _httpClient.PostAsJsonAsync("api/CareLogs", logDto);
            return response.IsSuccessStatusCode;
        }
    }
}