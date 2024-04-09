using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TMS.Services.Models
{
    public class UserVM
    {
        public string? Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public List<TaskVM>? Tasks { get; set; }
        public List<GroupVM>? Groups { get; set; }
        public TMS.Data.Enums.UserRole? Role { get; set; }
    }
}
