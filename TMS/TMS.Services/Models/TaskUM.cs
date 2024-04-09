using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.Services.Models
{
    public class TaskUM
    {
        public string? TaskId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? DueDate { get; set; }
        public string? UserId { get; set; }
        public string? GroupId { get; set; }
        public TMS.Data.Enums.TaskCategory? Category { get; set; }
        public TMS.Data.Enums.TaskPriority? Priority { get; set; }
        public TMS.Data.Enums.TaskStatus? Status { get; set; }
    }
}
