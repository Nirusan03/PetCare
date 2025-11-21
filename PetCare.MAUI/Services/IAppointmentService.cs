using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PetCare.Shared;
using PetCare.Shared.DTOs;

namespace PetCare.MAUI.Services
{
    public interface IAppointmentService
    {
        Task<List<Appointment>> GetAppointmentsByUserAsync(int userId);
        Task<bool> AddAppointmentAsync(CreatAppointmentDto appointmentDto);

        Task DeleteAppointmentAsync(int appointmentId);
        Task UpdateStatusAsync(int appointmentId, bool isCompleted);
    }
}