using System.ComponentModel.DataAnnotations;
using TMS.Data.Enums;

namespace TMS.WebHost.Models
{
    public class UserIM
    {
        public string? Id { get; set; }

        [Required(ErrorMessage = "Първото име е задължително")]
        [MaxLength(50, ErrorMessage = "Първото име не може да е над 50 символа")]
        [MinLength(2, ErrorMessage = "Първото име не може да е под 2 символа")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Фамилното име е задължително")]
        [MaxLength(50, ErrorMessage = "Фамилното име не може да е над 50 символа")]
        [MinLength(2, ErrorMessage = "Фамилното име не може да е под 2 символа")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Потребителското име е задължително")]
        [MaxLength(50, ErrorMessage = "Потребителското име не можде да е над 50 символа")]
        [MinLength(2, ErrorMessage = "Потребителското име не може да е под 2 символа")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Имейлът е задължителен")]
        [EmailAddress(ErrorMessage = "Невалиден имейл адрес")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Паролата е задължителна")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Ролята е задължителна")]
        public UserRole Role { get; set; }

        [Display(Name = "Снимка")]
        [Required(ErrorMessage = "Снимката е задължителна")]
        public IFormFile Image { get; set; }
    }
}
