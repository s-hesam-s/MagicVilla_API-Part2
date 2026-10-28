using System.ComponentModel.DataAnnotations;

namespace MagicVilla_VillaAPI.Models.Dto
{
    public class VillaCreateDTO
    {
        [Required]
        [MaxLength(30)]
        public string Name { get; set; }
        public string Details { get; set; }
        [Required]
        [Range(0.01, double.MaxValue)]
        public double Rate { get; set; }
        [Range(1, int.MaxValue)]
        public int Occupancy { get; set; }
        [Range(1, int.MaxValue)]
        public int Sqft { get; set; }
        public string? ImageUrl { get; set; }
        public IFormFile? Image { get; set; }
        public string Amenity { get; set; }
    }
}
