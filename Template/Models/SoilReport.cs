using System.ComponentModel.DataAnnotations;

namespace Template.Models
{
    public class SoilReport
    {
        [Key]
        public int SoilId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Region { get; set; }

        [Required]
        [MaxLength(50)]
        public string SoilType { get; set; }

        public decimal? pH { get; set; }

        public int? FertilityScore { get; set; }  // 1-10

        [MaxLength(500)]
        public string? RecommendedCrops { get; set; }

        [MaxLength(500)]
        public string? Recommendations { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public int AddedBy { get; set; }  // Admin UserId
    }
}