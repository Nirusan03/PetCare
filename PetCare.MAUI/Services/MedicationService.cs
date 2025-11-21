using System.Net.Http.Json;
using System.Net.Http.Headers;
using PetCare.Shared;
using PetCare.Shared.DTOs;

namespace PetCare.MAUI.Services
{
    public class MedicationService : IMedicationService
    {
        private readonly HttpClient _httpClient;
        private readonly UserState _userState;

        public MedicationService(HttpClient httpClient, UserState userState)
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

        public async Task<List<Medication>> GetMedicationsAsync(int petId)
        {
            SetAuthorizationHeader();
            try
            {
                return await _httpClient.GetFromJsonAsync<List<Medication>>($"api/Medications/pet/{petId}")
                       ?? new List<Medication>();
            }
            catch
            {
                return new List<Medication>();
            }
        }

        public async Task<bool> AddMedicationAsync(CreateMedicationDto medDto)
        {
            SetAuthorizationHeader();
            var response = await _httpClient.PostAsJsonAsync("api/Medications", medDto);
            return response.IsSuccessStatusCode;
        }

        public async Task StopMedicationAsync(int medicationId)
        {
            SetAuthorizationHeader();
            await _httpClient.PutAsync($"api/Medications/stop/{medicationId}", null);
        }
    }
}