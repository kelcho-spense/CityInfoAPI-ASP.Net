using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CityInfo.API.Entities
{
    public class PointOfInterest
    {

        [Key] //ensure Id property is PK
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]   //identity = key will be created during inserting. Computed = key be created during inserting & updating
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(200)]
        public string Description { get; set; }

        [ForeignKey("CityId")] //create a relationship btw PointOfInterest & City entities.
        public int CityId { get; set; } 
        public City? City { get; set; }

        public PointOfInterest(String name)
        {
            Name = name;
        }
    }
}
