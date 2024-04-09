namespace TMS.Data.Models
{
    public class Group
    {
        public string? GroupId { get; set; }
        public string? GroupName { get; set; }
        public ICollection<Task>? Tasks { get; set; }
        public ICollection<User>? Users { get; set; }
    }
}
