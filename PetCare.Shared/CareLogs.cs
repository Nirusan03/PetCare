using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace PetCare.Shared
{
    internal class CareLogs
    {
        [Key]
        public int CareLogId { get; set; }

        public int PetId { get; set; }
        [ForeignKey("PetId")]
        public Pet? Pet { get; set; }

        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User? User { get; set; }

        [Required]
        [MaxLength(50)]
        public string Type { get; set; }

        [Required]
        public DateTime TimeStamp { get; set; }

        public string Details { get; set; }

        [Required]
        [MaxLength(500)]
        public string PhotoUrl { get; set; }
    }
}
