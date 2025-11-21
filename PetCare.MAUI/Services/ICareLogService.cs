using PetCare.Shared;
using PetCare.Shared.DTOs;

namespace PetCare.MAUI.Services
{
    public interface ICareLogService
    {
        Task<List<CareLog>> GetLogsAsync(int petId);
        Task<bool> AddLogAsync(CreateCareLogDto logDto);
    }
}