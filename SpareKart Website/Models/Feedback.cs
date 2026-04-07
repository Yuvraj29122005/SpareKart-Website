using System;
using System.ComponentModel.DataAnnotations;

namespace SpareKart_Website.Models
{
    public class Feedback
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Message { get; set; }

        public DateTime DateSubmitted { get; set; } = DateTime.Now;
    }
}
