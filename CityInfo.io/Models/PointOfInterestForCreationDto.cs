using System.ComponentModel.DataAnnotations;

namespace CityInfo.io.Models
{
    public class PointOfInterestForCreationDto
    {
        [Required(ErrorMessage ="You should provide the name property")]
        [MaxLength(20)]
        public string Name { get; set; } = string.Empty;
        [Required]
        [MaxLength(100)]
        public string? Description { get; set; }
    }
}
