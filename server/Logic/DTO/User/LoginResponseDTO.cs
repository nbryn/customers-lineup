using System.ComponentModel.DataAnnotations;

namespace Logic.DTO.User
{
    public class LoginResponseDTO
    {
        public int Id { get; set; }
        [Required]
        public string Email { get; set; }

        public string Token {get; set;}
    }
}