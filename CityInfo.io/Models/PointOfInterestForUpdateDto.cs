using System.ComponentModel.DataAnnotations;

namespace CityInfo.io.Models
{
    public class PointOfInterestForUpdateDto
    {
        [Required(ErrorMessage = "You should provide the name property")]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;
       
        [MaxLength(100)]
        public string? Description { get; set; }
    }
}
