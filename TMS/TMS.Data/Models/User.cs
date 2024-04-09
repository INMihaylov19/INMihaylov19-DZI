using Microsoft.AspNetCore.Identity;
using TMS.Data.Enums;

namespace TMS.Data.Models
{
    public class User : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public UserRole? Role { get; set; }
        public ICollection<Task>? Tasks { get; set; }
        public ICollection<Group>? Groups { get; set; }
    }
}
