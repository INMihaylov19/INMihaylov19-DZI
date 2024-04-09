using System.ComponentModel.DataAnnotations;

namespace TMS.WebHost.Models.Account
{
    public class ForgotPasswordVM
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }

}
