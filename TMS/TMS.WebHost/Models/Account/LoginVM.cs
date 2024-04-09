using System.ComponentModel.DataAnnotations;

namespace TMS.WebHost.Models.Account
{
    public class LoginVM
    {
        [Display(Name = "Имейл")]
        [Required(ErrorMessage = "Имейлът е задължителен")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Паролата е задължителна")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
    }
}
