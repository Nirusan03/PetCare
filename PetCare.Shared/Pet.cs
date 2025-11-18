using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace PetCare.Shared
{
    internal class Pet
    {
        [Key]
        public int PetId { get; set; }

        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User? Owner { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [MaxLength(50)]
        public string Species { get; set; }

        [Required]
        [MaxLength(100)]
        public string Breed { get; set; }

        [Required]
        [MaxLength(10)]
        public string Sex { get; set; }

        [Required]
        [MaxLength(500)]
        public string PhotoUrl { get; set; }

        public string Notes { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
