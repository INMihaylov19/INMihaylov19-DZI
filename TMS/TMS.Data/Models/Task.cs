using TMS.Data.Enums;

namespace TMS.Data.Models
{
    public class Task
    {
        public string? TaskId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? DueDate { get; set; }
        public string? UserId { get; set; }
        public User? User { get; set; }
        public string? GroupId { get; set; }
        public Group? Group { get; set; }
        public TaskCategory? Category { get; set; }
        public TaskPriority? Priority { get; set; }
        public TMS.Data.Enums.TaskStatus? Status { get; set; }
        public DateTime? CreatedOn { get; set; }
    }
}
