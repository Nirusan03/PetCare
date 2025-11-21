using System.Net.Http.Json;
using System.Net.Http.Headers;
using PetCare.Shared;
using PetCare.Shared.DTOs;

namespace PetCare.MAUI.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly HttpClient _httpClient;
        private readonly UserState _userState;

        public AppointmentService(HttpClient httpClient, UserState userState)
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

        public async Task<List<Appointment>> GetAppointmentsByUserAsync(int userId)
        {
            SetAuthorizationHeader();
            try
            {
                return await _httpClient.GetFromJsonAsync<List<Appointment>>($"api/Appointments/user/{userId}")
                       ?? new List<Appointment>();
            }
            catch
            {
                return new List<Appointment>();
            }
        }

        public async Task<bool> AddAppointmentAsync(CreatAppointmentDto appointmentDto)
        {
            SetAuthorizationHeader();
            // This sends the DTO as JSON to the controller
            var response = await _httpClient.PostAsJsonAsync("api/Appointments", appointmentDto);
            return response.IsSuccessStatusCode;
        }

        public async Task DeleteAppointmentAsync(int appointmentId)
        {
            SetAuthorizationHeader();
            await _httpClient.DeleteAsync($"api/Appointments/{appointmentId}");
        }

        public async Task UpdateStatusAsync(int appointmentId, bool isCompleted)
        {
            SetAuthorizationHeader();
            var dto = new UpdateAppointmentStatusDto
            {
                AppointmentId = appointmentId,
                IsCompleted = isCompleted
            };

            // Note: We use PutAsJsonAsync because we changed the API to [HttpPut]
            await _httpClient.PutAsJsonAsync("api/Appointments/status", dto);
        }
    }
}