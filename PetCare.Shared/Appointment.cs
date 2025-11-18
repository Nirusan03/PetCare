using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetCare.Shared
{
    internal class Appointment
    {
        [Key]
        public int AppointmentId { get; set; }

        public int PetId { get; set; }
        [ForeignKey("PetId")]
        public Pet ? Pets { get; set; }

        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User ? User { get; set; }

        [Required]
        [MaxLength(200)]
        public string Provider { get; set; }

        [Required]
        [MaxLength(50)]
        public string Type { get; set; }

        [Required]
        public DateTime StartedDateTime { get; set; }

        [Required]
        public DateTime EndDateTime { get; set; }

        public string Text { get; set; }

        [Required]
        [MaxLength(20)]
        public string Status { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
