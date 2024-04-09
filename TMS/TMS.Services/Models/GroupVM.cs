using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.Services.Models
{
    public class GroupVM
    {
        public string? GroupId { get; set; }
        public string? GroupName { get; set; }
        public string? UserId { get; set; }
        public List<TaskVM>? Tasks { get; set; }
        public List<UserVM>? Users { get; set; }
    }
}
