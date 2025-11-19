using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace PetCare.Shared
{
    public class Reminder
    {
        [Key]
        public int ReminderId { get; set; }

        public int PetId { get; set; }
        [ForeignKey("PetId")]
        public Pet? Pet { get; set; }

        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User? User { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime RemindAt { get; set; }

        [Required]
        [MaxLength(50)]
        public string Repeat { get; set; }

        public int IsSent { get; set; } = 0;
    }
}
