using System.ComponentModel.DataAnnotations;

namespace TMS.WebHost.Models
{
    public class GroupIM
    {
        [Required(ErrorMessage = "Името на групата е задължително")]
        [MinLength(2, ErrorMessage = "Името на групата не може да е под 2 символа")]
        [MaxLength(50, ErrorMessage = "Името на групата не може да е над 50 символа")]
        public string GroupName { get; set; }

        public IEnumerable<string>? UserId { get; set; }
    }
}
