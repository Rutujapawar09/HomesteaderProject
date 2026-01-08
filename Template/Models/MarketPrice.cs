using System.ComponentModel.DataAnnotations;

namespace Template.Models
{
    public class MarketPrice
    {
        [Key]
        public int PriceId { get; set; }

        [Required]
        [MaxLength(100)]
        public string CropName { get; set; }

        [Required]
        [MaxLength(100)]
        public string MarketName { get; set; }

        [Required]
        public decimal Price { get; set; }

        [MaxLength(20)]
        public string Unit { get; set; } = "Quintal";

        [Required]
        public DateTime Date { get; set; } = DateTime.Now;

        public int AddedBy { get; set; }  // Admin UserId
    }
}