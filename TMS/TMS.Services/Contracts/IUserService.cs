
using TMS.Services.Models;

namespace TMS.Services.Contracts
{
    public interface IUserService
    {
        Task UpdateUserAsync(string userId, TMS.Services.Models.UserUM user);
        Task<TMS.Services.Models.UserVM> CreateUserAsync(TMS.Data.Models.User user);
        Task<List<TMS.Services.Models.UserVM>> GetAllUsersAsync();
        Task<TMS.Services.Models.UserVM> GetUserByIdAsync(string userId);
        Task DeleteUserAsync(string userId);
        Task<List<UserVM>> GetUserByUsernameAsync(string userName);
        Task<TMS.Data.Models.User> GetUserByEmailAsync(string email);
        Task<List<UserVM>> GetUserByFirstNameAsync(string firstName);
        Task<List<UserVM>?> GetUserByRoleAsync(string searchValue);
        UserVM GetTasksFofCurrentUserAsync();
        UserVM GetGroupsOfCurrentUser();
        bool HasClaim(string claimType, string claimValue);
    }
}
