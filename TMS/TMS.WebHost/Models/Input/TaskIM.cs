using System.ComponentModel.DataAnnotations;
using TMS.Data.Enums;

namespace TMS.WebHost.Models
{
    public class TaskIM
    {

        [Required(ErrorMessage = "Името на задачата е задължително")]
        [MinLength(2, ErrorMessage = "Името на задачата не може да е под 2 символа")]
        [MaxLength(50, ErrorMessage = "Името на задачата не може да е над 50 символа")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Описанието на задачата е задължително")]
        [MinLength(2, ErrorMessage = "Описанието на задачата не може да е под 2 символа")]
        [MaxLength(250, ErrorMessage = "Описанието на задачата не може да е над 250 символа")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Началната дата е задължителна")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Крайната дата е задължителна")]
        public DateTime DueDate { get; set; }

        [Required(ErrorMessage = "Категорията е задължителна")]
        public TaskCategory Category { get; set; }

        [Required(ErrorMessage = "Приоритета е задължителен")]
        public TaskPriority Priority { get; set; }
        public TMS.Data.Enums.TaskStatus? Status { get; set; }
        public DateTime? CreatedOn { get; set; }
    }
}
