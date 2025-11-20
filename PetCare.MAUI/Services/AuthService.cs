using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Json;
using PetCare.Shared.DTOs;

namespace PetCare.MAUI.Services
{
    public class AuthService: IAuthService
    {
        private readonly HttpClient _httpClient;
        public AuthService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest loginRequest)
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/login", loginRequest);

            if(response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
                if(result != null && !string.IsNullOrEmpty(result.Token))
                {
                    await SecureStorage.SetAsync("auth_token", result.Token);
                    await SecureStorage.SetAsync("username", loginRequest.Email);
                    return result;
                }
                else
                {
                    throw new Exception("Invalid login response.");
                }
            }
            return null;
        }

        public async Task<bool> RegisterAsync(RegisterRequest registerRequest)
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/register", registerRequest);
            return response.IsSuccessStatusCode;
        }

        public async Task LogoutAsync()
        {
            SecureStorage.Remove("auth_token");
            SecureStorage.Remove("username");
            await Task.CompletedTask;
        }
    }
}
