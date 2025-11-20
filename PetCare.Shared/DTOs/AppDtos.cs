using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetCare.Shared.DTOs
{
    // Data Transfer Object for creating a new appointment
    public class CreatAppointmentDto
    {
        public int PetId { get; set; }
        public DateTime AppointmentTime { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
    }

    // Data Transfer Object for updating an appointment
    public class UpdateAppointmentStatusDto
    {
        public int AppointmentId { get; set; }
        public bool IsCompleted { get; set; }
    }

    // Data Transfer Object for creating Medication
    public class CreateMedicationDto
    {
        public int PetId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Dosage { get; set; } = string.Empty;
        public string Frequency { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    // Data Transfer Object for creating care logs
    public class  CreateCareLogDto
    {
        public int PetId { get; set; }
        public string ActivityType { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public DateTime LogDate { get; set; }
    }

    // Data Transfer Object for creating Reminders
    public class CreateReminderDto
    {
        public int UserId { get; set; }
        public int PetId { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime ReminderDate { get; set; }
        public bool IsRecurring { get; set; }
    }
}
