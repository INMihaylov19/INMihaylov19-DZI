using TMS.Services.Models;

namespace TMS.WebHost.Models
{
    public class AssignUserToGroupVM
    {
        public GroupVM? Group { get; set; }
        public IEnumerable<UserVM>? Users { get; set; }
    }
}
