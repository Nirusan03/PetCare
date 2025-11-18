using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace PetCare.Shared
{
    internal class PetSharedAccess
    {
        [Key]
        public int ShareID { get; set; }

        public int PetId { get; set; }
        [ForeignKey("PetId")]
        public Pet ? Pet { get; set; }

        public int OwnerUserId { get; set; }
        [ForeignKey("OwnerUserId")]
        public User ? OwnerUser { get; set; }

        [Required]
        [MaxLength(100)]
        public string Permission { get; set; }
    }
}
