using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetCare.Shared
{
    public class Medication
    {
        [Key]
        public int MedicationId { get; set; }

        public int PetId;
        [ForeignKey("PetId")]
        public Pet? Pet { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        [Required]
        [MaxLength(100)]
        public string Dosage { get; set; }

        [Required]
        [MaxLength(100)]
        public string Frequency { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }
        public string Notes { get; set; }
    }
}
