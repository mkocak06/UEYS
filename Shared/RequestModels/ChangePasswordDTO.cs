using System.ComponentModel.DataAnnotations;

namespace Shared.RequestModels
{
    public class ChangePasswordDTO
    {
        [Required]
        [StringLength(20, MinimumLength = 4, ErrorMessage = "You must specify a password between 4 and 20 characters")]
        public string CurrentPassword { get; set; } = string.Empty;
        [Required]
        [StringLength(20, MinimumLength = 4, ErrorMessage = "You must specify a password between 4 and 20 characters")]
        public string Password { get; set; } = string.Empty;
        [Required]
        [Compare(nameof(Password), ErrorMessage = "Passwords don't match")]
        public string RepeatPassword { get; set; } = string.Empty;
    }
}
