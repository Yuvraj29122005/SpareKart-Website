using System.ComponentModel.DataAnnotations;

namespace SpareKart_Website.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string? Phone { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        public bool IsEmailVerified { get; set; } = false;
        
        public string? Otp { get; set; }
        
        public DateTime? OtpExpiry { get; set; }

        public string? ProfilePictureUrl { get; set; }

        public string? Address { get; set; }
    }
}
