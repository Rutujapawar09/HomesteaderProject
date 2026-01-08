using System.ComponentModel.DataAnnotations;

namespace Template.Models
{
    public class Fertilizer
    {
        [Key]
        public int FertilizerId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string? Usage { get; set; }

        [MaxLength(200)]
        public string? Dosage { get; set; }

        [MaxLength(500)]
        public string? Benefits { get; set; }

        [MaxLength(200)]
        public string? SuitableFor { get; set; }  // Crop names

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public int AddedBy { get; set; }  // Admin UserId
    }
}