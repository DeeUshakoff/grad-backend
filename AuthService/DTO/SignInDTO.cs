﻿using System.ComponentModel.DataAnnotations;

namespace AuthService.DTO
{
    public class SignInDTO
    {
        [EmailAddress]
        [Required] public required string Email { get; set; }
        [Required] public required string Password { get; set; }

    }
}
