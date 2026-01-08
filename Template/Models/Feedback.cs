using System.ComponentModel.DataAnnotations;

namespace Template.Models
{
    public class Feedback
    {
        [Key]
        public int FeedbackId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Subject { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Message { get; set; }

        public int? Rating { get; set; }  // 1-5 stars

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [MaxLength(20)]
        public string Status { get; set; } = "Unread";  // Unread, Read, Responded
    }
}