using System.ComponentModel.DataAnnotations;

namespace TMS.WebHost.Models.Account
{
    public class RegisterVM
    {
        [Display(Name = "Първо име")]
        [Required(ErrorMessage = "Първото име е задължително")]
        [MaxLength(50, ErrorMessage = "Първото име не може да бъде над 50 символа")]
        [MinLength(2, ErrorMessage = "Първото име не може да бъде под 2 симовла")]
        public string? FirstName { get; set; }

        [Display(Name = "Фамилно име")]
        [Required(ErrorMessage = "Фамилното име е задължително")]
        [MaxLength(50, ErrorMessage = "Фамилното име не може да бъде над 50 символа")]
        [MinLength(2, ErrorMessage = "Фамилното име не може да бъде под 2 символа")]
        public string? LastName { get; set; }

        [Display(Name = "Потребителско име")]
        [Required(ErrorMessage = "Потребителското име е задължително")]
        [MaxLength(50, ErrorMessage = "Потребителското име не може да бъде над 50 символа")]
        [MinLength(2, ErrorMessage = "Потребителското име не може да бъде под 2 символа")]
        public string? UserName { get; set; }

        [Display(Name = "Имейл адрес")]
        [Required(ErrorMessage = "Имейл адресът е задължителен")]
        public string? Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Display(Name = "Потвърждаване на парола")]
        [Required(ErrorMessage = "Паролата за потвърждаване е задължителна")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Паролата не съвпада")]
        public string? ConfirmPassword { get; set; }
    }
}
