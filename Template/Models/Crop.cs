using System.ComponentModel.DataAnnotations;

namespace Template.Models
{
    public class Crop
    {
        [Key]
        public int CropId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        [MaxLength(50)]
        public string? Season { get; set; }  // Kharif, Rabi, Zaid

        [MaxLength(100)]
        public string? SoilType { get; set; }

        [MaxLength(100)]
        public string? Region { get; set; }

        public int? WaterRequirement { get; set; }  // in mm

        public int? GrowthDuration { get; set; }  // in days

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public int CreatedBy { get; set; }  // Admin UserId
    }
}