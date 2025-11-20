using PetCare.Shared;
using PetCare.Shared.DTOs;

namespace PetCare.MAUI.Services
{
    public interface IPetService
    {
        Task<List<Pet>> GetPetsByUserAsync(int userId);
        Task<Pet?> GetPetDetailsAsync(int petId);
        Task<bool> AddPetAsync(CreatePetDto petDto);
        Task DeletePetAsync(int petId);
    }
}