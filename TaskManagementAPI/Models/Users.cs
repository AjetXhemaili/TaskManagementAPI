using System.ComponentModel.DataAnnotations;

namespace TaskManagementAPI.Models
{
    public class Users
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Username { get; set; } = string.Empty;

        [Required, StringLength(255)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        public string Role { get; set; } = "User"; // "User" or "Admin"

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
