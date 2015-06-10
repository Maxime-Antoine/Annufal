using System.ComponentModel.DataAnnotations;

namespace Annufal.Authentication
{
    public class CreateUserBindingModel
    {
        [Required]
        [EmailAddress]
        [MaxLength(100, ErrorMessage="Email must be no more than 100 characters long")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [MinLength(3, ErrorMessage="Username must be at least 3 characters long")]
        [MaxLength(50, ErrorMessage="Username must be no more than 50 characters long")]
        [Display(Name = "Username")]
        public string Username { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}