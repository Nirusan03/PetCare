using PetCare.Shared;
using PetCare.Shared.DTOs;

namespace PetCare.MAUI.Services
{
    public interface IMedicationService
    {
        Task<List<Medication>> GetMedicationsAsync(int petId);
        Task<bool> AddMedicationAsync(CreateMedicationDto medDto);
        Task StopMedicationAsync(int medicationId);
    }
}