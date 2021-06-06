using System.ComponentModel.DataAnnotations;
namespace CLup.Businesses.DTO
{
    public class BusinessRequest
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        [Required]
        public string Zip { get; set; }
        [Required]
        public string Address { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public string OwnerEmail { get; set; }
        [Required]
        public string Opens { get; set; }
        [Required]
        public string Closes { get; set; }

        [Required]
        public int Capacity { get; set; }
        [Required]
        public int TimeSlotLength { get; set; }

        [Required]
        public string Type { get; set; }
    }
}