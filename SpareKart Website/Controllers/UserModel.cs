using System.ComponentModel.DataAnnotations;

namespace SpareKart_Website.Models
{
    public class UserModel
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public static List<UserModel> users = new List<UserModel>();
    }
}