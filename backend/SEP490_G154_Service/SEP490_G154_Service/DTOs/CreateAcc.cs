using System.ComponentModel.DataAnnotations;

namespace SEP490_G154_Service.DTOs
{
    public class CreateAcc
    {

        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@(gmail\.com|fpt\.edu\.vn)$",
                ErrorMessage = "Email must end with @gmail.com or @fpt.edu.vn")]
        public string Email { get; set; }

        [Required]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\W).+$",
            ErrorMessage = "Password must contain uppercase, lowercase, and special character")]
        public string Password { get; set; }

        [Required]
        public string FullName { get; set; }

        [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone must contain exactly 10 digits")]
        public string? Phone { get; set; }
    }
}
