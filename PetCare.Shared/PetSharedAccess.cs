using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace PetCare.Shared
{
    public class PetSharedAccess
    {
        [Key]
        public int ShareId { get; set; }

        public int PetId { get; set; }
        [ForeignKey("PetId")]
        public Pet ? Pet { get; set; }

        public int OwnerUserId { get; set; }
        [ForeignKey("OwnerUserId")]
        public User ? Owner { get; set; }

        [Required]
        [MaxLength(100)]
        public string Permission { get; set; }
    }
}
