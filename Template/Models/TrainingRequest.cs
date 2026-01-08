using System.ComponentModel.DataAnnotations;

namespace Template.Models
{
    public class TrainingRequest
    {
        [Key]
        public int RequestId { get; set; }

        [Required]
        public int UserId { get; set; }  // Farmer or Student

        [Required]
        [MaxLength(200)]
        public string Topic { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "Pending";  // Pending, Approved, Completed, Rejected

        public int? AssignedTo { get; set; }  // Agency UserId

        public DateTime RequestDate { get; set; } = DateTime.Now;

        public DateTime? ResponseDate { get; set; }

        [MaxLength(500)]
        public string? Response { get; set; }
    }
}