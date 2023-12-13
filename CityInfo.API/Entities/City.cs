using CityInfo.API.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CityInfo.API.Entities
{
    public class City
    {
        [Key] //ensure Id property is PK
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]   //identity = key will be created during inserting. Computed = key be created during inserting & updating
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        [MaxLength(200)]
        public string? Description { get; set; }
        
        public ICollection<PointOfInterest> PointsOfInterest { get; set; } = 
            new List<PointOfInterest>(); // return a list of pointsofinterest
        public City( String name)
        {
            Name = name;
        } 
    }   
}
