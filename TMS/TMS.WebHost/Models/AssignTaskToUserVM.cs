using TMS.Services.Models;

namespace TMS.WebHost.Models
{
    public class AssignTaskToUserVM
    {
        public TaskVM? Task { get; set; }
        public IEnumerable<UserVM>? Users { get; set; }
    }
}
