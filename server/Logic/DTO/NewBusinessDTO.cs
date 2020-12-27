using System.ComponentModel.DataAnnotations;

namespace Logic.DTO
{
    public class NewBusinessDTO
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        public int Zip { get; set; }

        [Required]
        public string Address { get; set; }

        public string OwnerEmail { get; set; }

        [Required]
        public string Opens { get; set; }

        [Required]
        public string Closes { get; set; }

        [Required]
        public int Capacity { get; set; }

        [Required]
        public int timeSlotLength { get; set; }

        [Required]
        public string Type { get; set; }

    }
}