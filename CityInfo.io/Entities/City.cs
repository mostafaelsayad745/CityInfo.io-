using CityInfo.io.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CityInfo.io.Entities
{
    public class City
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]

        public string Name { get; set; }
        public string? Description { get; set; }

        public ICollection<PointOfInterest> pointOfInterests { get; set; } = new List<PointOfInterest>();

        public City(string name)
        {
            Name = name;
        }
    }

    
}
