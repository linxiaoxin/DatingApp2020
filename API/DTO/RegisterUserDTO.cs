using System.ComponentModel.DataAnnotations;

namespace API.DTO
{
    public class RegisterUserDTO
    {
        [Required]
        public string Username { get; set; }
        [Required]
        [MaxLength(8)]
        [MinLength(4)]
        public string Password { get; set; }
    }
}