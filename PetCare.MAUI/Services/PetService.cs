using PetCare.Shared;
using PetCare.Shared.DTOs;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace PetCare.MAUI.Services
{
    public class PetService : IPetService
    {
        private readonly HttpClient _httpClient;
        private readonly UserState _userState;

        public PetService(HttpClient httpClient, UserState userState)
        {
            _httpClient = httpClient;
            _userState = userState;
        }

        // Helper to attach the token before every request
        private void SetAuthorizationHeader()
        {
            if (!string.IsNullOrEmpty(_userState.AuthToken))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", _userState.AuthToken);
            }
        }

        public async Task<List<Pet>> GetPetsByUserAsync(int userId)
        {
            SetAuthorizationHeader();
            try
            {
                // The endpoint is /api/Pets/user/{id}
                return await _httpClient.GetFromJsonAsync<List<Pet>>($"api/Pets/user/{userId}") ?? new List<Pet>();
            }
            catch
            {
                return new List<Pet>();
            }
        }

        public async Task<bool> AddPetAsync(CreatePetDto petDto)
        {
            SetAuthorizationHeader();
            var response = await _httpClient.PostAsJsonAsync("api/Pets", petDto);
            return response.IsSuccessStatusCode;
        }

        public async Task<Pet?> GetPetDetailsAsync(int petId)
        {
            SetAuthorizationHeader();
            try
            {
                return await _httpClient.GetFromJsonAsync<Pet>($"api/Pets/{petId}");
            }
            catch
            {
                return null;
            }
        }

        public async Task DeletePetAsync(int petId)
        {
            SetAuthorizationHeader();
            await _httpClient.DeleteAsync($"api/Pets/{petId}");
        }
    }
}