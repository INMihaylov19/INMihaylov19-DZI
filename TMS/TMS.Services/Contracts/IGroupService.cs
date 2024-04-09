
using TMS.Services.Models;

namespace TMS.Services.Contracts
{
    public interface IGroupService
    {
        Task<List<TMS.Services.Models.GroupVM>> GetAllGroupsAsync();
        Task<TMS.Services.Models.GroupVM> GetGroupByIdAsync(string groupId);
        Task<TMS.Services.Models.GroupVM> CreateGroupAsync(TMS.Data.Models.Group group);
        Task UpdateGroupAsync(string groupId, TMS.Services.Models.GroupUM group);
        Task DeleteGroupAsync(string groupId);
        Task<List<GroupVM>> GetGroupdByNameAsync(string groupName);
        Task AssignUserToGroupAsync(string userId, string groupId);
        Task RemoveUserFromGroup(string userId, string groupId);
        Task<List<UserVM>> GetUsersInGroupAsync(string groupId);
    }
}
