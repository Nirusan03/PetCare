namespace PetCare.Shared.DTOs
{
    public class CreatePetDto
    {
        public int UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Species { get; set; } = string.Empty;
        public string Breed { get; set; } = string.Empty;
        public DateTime DOB { get; set; }
        public string Sex { get; set; } = string.Empty;
        public string PhotoUrl { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
    }
}
