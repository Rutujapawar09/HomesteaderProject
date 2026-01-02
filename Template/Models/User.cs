using System.ComponentModel.DataAnnotations;

namespace Template.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [MaxLength(100)]
        public string Email { get; set; }

        [Required]
        [MaxLength(100)]
        public string Password { get; set; }

        [Required]
        [MaxLength(20)]
        public string Role { get; set; }  // SuperAdmin, Admin, Farmer, Student, Agency

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected

        public int? ApprovedBy { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
