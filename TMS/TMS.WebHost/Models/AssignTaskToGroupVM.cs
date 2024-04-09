using TMS.Services.Models;

namespace TMS.WebHost.Models
{
    public class AssignTaskToGroupVM
    {
        public TaskVM? Task { get; set; }
        public IEnumerable<GroupVM>? Groups { get; set; }
    }
}
