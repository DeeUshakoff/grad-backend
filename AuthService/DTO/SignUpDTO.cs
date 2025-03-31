using System.ComponentModel.DataAnnotations;

namespace AuthService.DTO
{
    public class SignUpDTO
    {
        [EmailAddress]
        [Required] public required string Email { get; set; }
        [Required] public required string Username { get; set; }
        [Required] public required string Password { get; set; }
        [Required] public required string RepeatPassword { get; set; }
    }
}
